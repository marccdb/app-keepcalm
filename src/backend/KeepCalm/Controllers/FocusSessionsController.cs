using Microsoft.AspNetCore.Mvc;
using KeepCalm.DTOs;
using KeepCalm.Services;

namespace KeepCalm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FocusSessionsController : ControllerBase
    {
        private readonly IFocusSessionService _focusService;
        private readonly ILogger<FocusSessionsController> _logger;

        public FocusSessionsController(IFocusSessionService focusService, ILogger<FocusSessionsController> logger)
        {
            _focusService = focusService;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartFocusSessionDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Error = "Validation failed", Details = GetValidationErrors() });

            var session = await _focusService.StartSessionAsync(dto, ct);
            _logger.LogInformation("Focus session started: {SessionId}", session.Id);
            return Ok(session);
        }

        [HttpPatch("{id}/pause")]
        public async Task<IActionResult> Pause(Guid id, CancellationToken ct = default)
        {
            var session = await _focusService.PauseSessionAsync(id, ct);
            return Ok(session);
        }

        [HttpPatch("{id}/resume")]
        public async Task<IActionResult> Resume(Guid id, CancellationToken ct = default)
        {
            var session = await _focusService.ResumeSessionAsync(id, ct);
            return Ok(session);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id, CancellationToken ct = default)
        {
            var session = await _focusService.CompleteSessionAsync(id, ct);
            return Ok(session);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int days = 30,
            CancellationToken ct = default)
        {
            var history = await _focusService.GetHistoryAsync(days, ct);
            return Ok(history);
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
