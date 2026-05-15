using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KeepCalm.Data;
using KeepCalm.DTOs;
using KeepCalm.Models.Entities;

namespace KeepCalm.Services
{
    public class TaskService : ITaskService
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(MongoDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TaskDto>> GetTasksAsync(string? priority = null, string? status = null, CancellationToken ct = default)
        {
            var query = _context.Tasks
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.OrderIndex)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<TaskPriority>(priority, out var parsedPriority))
            {
                query = query.Where(t => t.Priority == parsedPriority);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskItemStatus>(status, out var parsedStatus))
            {
                query = query.Where(t => t.Status == parsedStatus);
            }

            var tasks = await query.ToListAsync(ct);

            // Load microsteps separately (MongoDB EF Core doesn't support Include for separate collections)
            var taskIds = tasks.Select(t => t.Id).ToList();
            var microSteps = await _context.MicroSteps
                .Where(ms => taskIds.Contains(ms.TaskId) && !ms.IsDeleted)
                .ToListAsync(ct);

            var microStepsByTask = microSteps.GroupBy(ms => ms.TaskId)
                .ToDictionary(g => g.Key, g => g.OrderBy(ms => ms.OrderIndex).ToList());

            foreach (var task in tasks)
            {
                if (microStepsByTask.TryGetValue(task.Id, out var steps))
                {
                    task.MicroSteps = steps;
                }
            }

            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken ct = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

            if (task == null) return null;

            // Load microsteps separately (MongoDB EF Core doesn't support Include for separate collections)
            var microSteps = await _context.MicroSteps
                .Where(ms => ms.TaskId == id && !ms.IsDeleted)
                .OrderBy(ms => ms.OrderIndex)
                .ToListAsync(ct);

            task.MicroSteps = microSteps;

            return MapToDto(task);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken ct = default)
        {
            // MongoDB EF Core doesn't support DefaultIfEmpty + MaxAsync
            var tasks = await _context.Tasks
                .Where(t => !t.IsDeleted)
                .ToListAsync(ct);

            int lastOrder = 0;
            if (tasks.Count > 0)
            {
                lastOrder = tasks.Max(t => t.OrderIndex);
            }

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                Priority = dto.Priority,
                Status = TaskItemStatus.Pending,
                OrderIndex = lastOrder + 1,
                Tags = dto.Folder != null ? new List<string> { dto.Folder } : new List<string>()
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Task created: {TaskId} - {Title}", task.Id, task.Title);

            return MapToDto(task);
        }

        public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, CancellationToken ct = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

            if (task == null)
                throw new KeyNotFoundException($"Task not found: {id}");

            task.Title = dto.Title.Trim();
            task.Description = dto.Description?.Trim();

            if (dto.Priority.HasValue)
                task.Priority = dto.Priority.Value;

            if (dto.Status.HasValue)
                task.Status = dto.Status.Value;

            if (dto.Tags != null)
                task.Tags = dto.Tags;

            task.UpdateTimestamp();
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Task updated: {TaskId} - {Title}", task.Id, task.Title);

            return MapToDto(task);
        }

        public async Task<bool> DeleteTaskAsync(Guid id, CancellationToken ct = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

            if (task == null)
                return false;

            task.SoftDelete();
            task.UpdateTimestamp();
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Task soft deleted: {TaskId}", task.Id);
            return true;
        }

        public async Task<List<TaskDto>> ReorderTasksAsync(List<Guid> orderedIds, CancellationToken ct = default)
        {
            var tasks = await _context.Tasks
                .Where(t => orderedIds.Contains(t.Id) && !t.IsDeleted)
                .ToListAsync(ct);

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var task = tasks.FirstOrDefault(t => t.Id == orderedIds[i]);
                if (task != null)
                {
                    task.OrderIndex = i;
                    task.UpdateTimestamp();
                }
            }

            await _context.SaveChangesAsync(ct);

            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<MicroStep>> GetMicroStepsAsync(Guid taskId, CancellationToken ct = default)
        {
            return await _context.MicroSteps
                .Where(ms => ms.TaskId == taskId && !ms.IsDeleted)
                .OrderBy(ms => ms.OrderIndex)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<MicroStep> AddMicroStepAsync(Guid taskId, CreateMicroStepDto dto, CancellationToken ct = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted, ct);

            if (task == null)
                throw new KeyNotFoundException($"Task not found: {taskId}");

            var maxOrder = await _context.MicroSteps
                .Where(ms => ms.TaskId == taskId && !ms.IsDeleted)
                .DefaultIfEmpty()
                .MaxAsync(ms => (int?)ms.OrderIndex, ct) ?? -1;

            var microStep = new MicroStep
            {
                TaskId = taskId,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                OrderIndex = maxOrder + 1
            };

            task.MicroSteps.Add(microStep);
            await _context.SaveChangesAsync(ct);

            return microStep;
        }

        public async Task<MicroStep> UpdateMicroStepAsync(Guid microStepId, UpdateMicroStepDto dto, CancellationToken ct = default)
        {
            var microStep = await _context.MicroSteps
                .FirstOrDefaultAsync(ms => ms.Id == microStepId && !ms.IsDeleted, ct);

            if (microStep == null)
                throw new KeyNotFoundException($"MicroStep not found: {microStepId}");

            if (dto.Title != null)
                microStep.Title = dto.Title.Trim();

            if (dto.Description != null)
                microStep.Description = dto.Description.Trim();

            if (dto.IsCompleted.HasValue)
                microStep.IsCompleted = dto.IsCompleted.Value;

            microStep.UpdateTimestamp();

            // Update parent task status
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == microStep.TaskId && !t.IsDeleted, ct);

            if (task != null)
            {
                var allDone = await _context.MicroSteps
                    .AllAsync(ms => ms.TaskId == task.Id && !ms.IsDeleted && ms.IsCompleted, ct);

                task.Status = allDone ? TaskItemStatus.Completed : TaskItemStatus.InProgress;
                task.UpdateTimestamp();
            }

            await _context.SaveChangesAsync(ct);
            return microStep;
        }

        private TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Status = task.Status,
                OrderIndex = task.OrderIndex,
                Tags = task.Tags,
                Progress = task.GetProgress(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                MicroSteps = task.MicroSteps
                    .Where(ms => !ms.IsDeleted)
                    .OrderBy(ms => ms.OrderIndex)
                    .Select(ms => new MicroStepDto
                    {
                        Id = ms.Id,
                        TaskId = ms.TaskId,
                        Title = ms.Title,
                        Description = ms.Description,
                        IsCompleted = ms.IsCompleted,
                        OrderIndex = ms.OrderIndex,
                        CreatedAt = ms.CreatedAt,
                        UpdatedAt = ms.UpdatedAt
                    })
                    .ToList()
            };
        }
    }
}
