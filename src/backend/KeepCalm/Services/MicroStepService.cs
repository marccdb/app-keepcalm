using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KeepCalm.Data;
using KeepCalm.DTOs;
using KeepCalm.Models.Entities;

namespace KeepCalm.Services
{
    public class MicroStepService : IMicroStepService
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<MicroStepService> _logger;

        public MicroStepService(MongoDbContext context, ILogger<MicroStepService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<MicroStepDto>> GetMicroStepsAsync(Guid taskId, CancellationToken ct = default)
        {
            return await _context.MicroSteps
                .Where(ms => ms.TaskId == taskId && !ms.IsDeleted)
                .OrderBy(ms => ms.OrderIndex)
                .AsNoTracking()
                .Select(ms => MapToDto(ms))
                .ToListAsync(ct);
        }

        public async Task<MicroStepDto> AddMicroStepAsync(Guid taskId, CreateMicroStepDto dto, CancellationToken ct = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted, ct);

            if (task == null)
                throw new KeyNotFoundException($"Task not found: {taskId}");

            var tasks = await _context.MicroSteps
                .Where(ms => ms.TaskId == taskId && !ms.IsDeleted)
                .ToListAsync(ct);

            int maxOrder = 0;
            if (tasks.Count > 0)
            {
                maxOrder = tasks.Max(ms => ms.OrderIndex);
            }

            var microStep = new MicroStep
            {
                TaskId = taskId,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                OrderIndex = maxOrder + 1
            };

            task!.MicroSteps.Add(microStep);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("MicroStep created: {MicroStepId} for Task: {TaskId}", microStep.Id, taskId);

            return MapToDto(microStep);
        }

        public async Task<MicroStepDto> UpdateMicroStepAsync(Guid microStepId, UpdateMicroStepDto dto, CancellationToken ct = default)
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
                var allSteps = await _context.MicroSteps
                    .Where(ms => ms.TaskId == task.Id && !ms.IsDeleted)
                    .ToListAsync(ct);

                var allDone = allSteps.Count > 0 && allSteps.All(ms => ms.IsCompleted);

                task.Status = allDone ? TaskItemStatus.Completed : TaskItemStatus.InProgress;
                task.UpdateTimestamp();
            }

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("MicroStep updated: {MicroStepId}", microStep.Id);

            return MapToDto(microStep);
        }

        public async Task<bool> DeleteMicroStepAsync(Guid microStepId, CancellationToken ct = default)
        {
            var microStep = await _context.MicroSteps
                .FirstOrDefaultAsync(ms => ms.Id == microStepId && !ms.IsDeleted, ct);

            if (microStep == null)
                return false;

            microStep.SoftDelete();
            microStep.UpdateTimestamp();
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("MicroStep soft deleted: {MicroStepId}", microStep.Id);
            return true;
        }

        public async Task<List<MicroStepDto>> ReorderMicroStepsAsync(Guid taskId, List<int> orderIndices, CancellationToken ct = default)
        {
            var microSteps = await _context.MicroSteps
                .Where(ms => ms.TaskId == taskId && !ms.IsDeleted)
                .ToListAsync(ct);

            for (int i = 0; i < orderIndices.Count && i < microSteps.Count; i++)
            {
                microSteps[i].OrderIndex = orderIndices[i];
                microSteps[i].UpdateTimestamp();
            }

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("MicroSteps reordered for Task: {TaskId}", taskId);

            return microSteps.Select(MapToDto).ToList();
        }

        public async Task<MicroStepDto?> GetMicroStepByIdAsync(Guid microStepId, CancellationToken ct = default)
        {
            var microStep = await _context.MicroSteps
                .FirstOrDefaultAsync(ms => ms.Id == microStepId && !ms.IsDeleted, ct);

            if (microStep == null) return null;
            return MapToDto(microStep);
        }

        private MicroStepDto MapToDto(MicroStep microStep)
        {
            return new MicroStepDto
            {
                Id = microStep.Id,
                TaskId = microStep.TaskId,
                Title = microStep.Title,
                Description = microStep.Description,
                IsCompleted = microStep.IsCompleted,
                OrderIndex = microStep.OrderIndex,
                CreatedAt = microStep.CreatedAt,
                UpdatedAt = microStep.UpdatedAt
            };
        }
    }
}
