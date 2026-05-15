using Microsoft.AspNetCore.Mvc;
using KeepCalm.DTOs;
using KeepCalm.Services;

namespace KeepCalm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? priority = null,
            [FromQuery] string? status = null,
            CancellationToken ct = default)
        {
            var tasks = await _taskService.GetTasksAsync(priority, status, ct);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
        {
            var task = await _taskService.GetTaskByIdAsync(id, ct);
            if (task == null)
                return NotFound(new ErrorResponse { Error = "Task not found" });

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed", Details = GetValidationErrors() });

            var task = await _taskService.CreateTaskAsync(dto, ct);
            _logger.LogInformation("Task created: {TaskId}", task.Id);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed", Details = GetValidationErrors() });

            var task = await _taskService.UpdateTaskAsync(id, dto, ct);
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        {
            var deleted = await _taskService.DeleteTaskAsync(id, ct);
            if (!deleted)
                return NotFound(new ErrorResponse { Error = "Task not found" });

            return NoContent();
        }

        [HttpPatch("{id}/reorder")]
        public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderTaskDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed" });

            var tasks = await _taskService.GetTasksAsync(ct: ct);
            var orderedIds = new List<Guid>();

            if (dto.NewIndex > 0)
            {
                orderedIds = tasks.Select(t => t.Id).ToList();
                var task = orderedIds.FirstOrDefault(tid => tid == id);
                orderedIds.Remove(task);
                orderedIds.Insert(dto.NewIndex, task);
            }
            else
            {
                orderedIds = new List<Guid> { id };
            }

            var reordered = await _taskService.ReorderTasksAsync(orderedIds, ct);
            return Ok(reordered);
        }

        [HttpGet("{id}/micro-steps")]
        public async Task<IActionResult> GetMicroSteps(Guid id, CancellationToken ct = default)
        {
            var steps = await _taskService.GetMicroStepsAsync(id, ct);
            return Ok(steps);
        }

        [HttpPost("{taskId}/micro-steps")]
        public async Task<IActionResult> AddMicroStep(Guid taskId, [FromBody] CreateMicroStepDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed", Details = GetValidationErrors() });

            var step = await _taskService.AddMicroStepAsync(taskId, dto, ct);
            return CreatedAtAction(nameof(GetMicroSteps), new { id = taskId }, step);
        }

        [HttpPatch("micro-steps/{microStepId}")]
        public async Task<IActionResult> UpdateMicroStep(Guid microStepId, [FromBody] UpdateMicroStepDto dto, CancellationToken ct = default)
        {
            var step = await _taskService.UpdateMicroStepAsync(microStepId, dto, ct);
            return Ok(step);
        }

        private string GetValidationErrors()
        {
            var errors = ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .Select(e => $"{e.Key}: {e.Value!.Errors.First().ErrorMessage}")
                .ToList();
            return string.Join("; ", errors);
        }
    }
}
