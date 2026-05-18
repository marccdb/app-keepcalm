using Microsoft.AspNetCore.Mvc;
using KeepCalm.DTOs;
using KeepCalm.Services;

namespace KeepCalm.Controllers
{
    [ApiController]
    [Route("api/micro-steps")]
    public class MicroStepsController : ControllerBase
    {
        private readonly IMicroStepService _microStepService;
        private readonly ILogger<MicroStepsController> _logger;

        public MicroStepsController(IMicroStepService microStepService, ILogger<MicroStepsController> logger)
        {
            _microStepService = microStepService;
            _logger = logger;
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetMicroSteps(Guid taskId, CancellationToken ct = default)
        {
            var steps = await _microStepService.GetMicroStepsAsync(taskId, ct);
            return Ok(steps);
        }

        [HttpPost("task/{taskId}")]
        public async Task<IActionResult> AddMicroStep(Guid taskId, [FromBody] CreateMicroStepDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed", Details = GetValidationErrors() });

            var step = await _microStepService.AddMicroStepAsync(taskId, dto, ct);
            _logger.LogInformation("MicroStep created: {MicroStepId} for Task: {TaskId}", step.Id, taskId);
            return CreatedAtAction(nameof(GetMicroSteps), new { taskId }, step);
        }

        [HttpPatch("{microStepId}")]
        public async Task<IActionResult> Update(Guid microStepId, [FromBody] UpdateMicroStepDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed", Details = GetValidationErrors() });

            var step = await _microStepService.UpdateMicroStepAsync(microStepId, dto, ct);
            return Ok(step);
        }

        [HttpDelete("task/{taskId}")]
        public async Task<IActionResult> DeleteByTask(Guid taskId, CancellationToken ct = default)
        {
            var steps = await _microStepService.GetMicroStepsAsync(taskId, ct);
            foreach (var step in steps)
            {
                await _microStepService.DeleteMicroStepAsync(step.Id, ct);
            }
            return NoContent();
        }

        [HttpDelete("{microStepId}")]
        public async Task<IActionResult> Delete(Guid microStepId, CancellationToken ct = default)
        {
            var deleted = await _microStepService.DeleteMicroStepAsync(microStepId, ct);
            if (!deleted)
                return NotFound(new ErrorResponse { Error = "MicroStep not found" });

            return NoContent();
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
