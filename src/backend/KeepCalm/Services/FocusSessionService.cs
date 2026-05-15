using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KeepCalm.Data;
using KeepCalm.DTOs;
using KeepCalm.Models.Entities;

namespace KeepCalm.Services
{
    public class FocusSessionService : IFocusSessionService
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<FocusSessionService> _logger;

        public FocusSessionService(MongoDbContext context, ILogger<FocusSessionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FocusSessionDto> StartSessionAsync(StartFocusSessionDto dto, CancellationToken ct = default)
        {
            // Cancel any active session first
            var active = await _context.FocusSessions
                .FirstOrDefaultAsync(s => s.Status == SessionStatus.Running && !s.IsDeleted, ct);

            if (active != null)
            {
                active.Pause();
                await _context.SaveChangesAsync(ct);
            }

            var session = new FocusSession
            {
                DurationMinutes = dto.DurationMinutes,
                Type = dto.Type,
                TaskId = dto.TaskId
            };
            session.Start();

            _context.FocusSessions.Add(session);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Focus session started: {SessionId} - {Type} - {Minutes}min",
                session.Id, session.Type, session.DurationMinutes);

            return MapToDto(session);
        }

        public async Task<FocusSessionDto> PauseSessionAsync(Guid sessionId, CancellationToken ct = default)
        {
            var session = await _context.FocusSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted, ct);

            if (session == null)
                throw new KeyNotFoundException($"Focus session not found: {sessionId}");

            if (session.Status != SessionStatus.Running)
                throw new InvalidOperationException("Session is not running");

            session.Pause();
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Focus session paused: {SessionId}", session.Id);
            return MapToDto(session);
        }

        public async Task<FocusSessionDto> ResumeSessionAsync(Guid sessionId, CancellationToken ct = default)
        {
            var session = await _context.FocusSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted, ct);

            if (session == null)
                throw new KeyNotFoundException($"Focus session not found: {sessionId}");

            if (session.Status != SessionStatus.Paused)
                throw new InvalidOperationException("Session is not paused");

            session.Resume();
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Focus session resumed: {SessionId}", session.Id);
            return MapToDto(session);
        }

        public async Task<FocusSessionDto> CompleteSessionAsync(Guid sessionId, CancellationToken ct = default)
        {
            var session = await _context.FocusSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted, ct);

            if (session == null)
                throw new KeyNotFoundException($"Focus session not found: {sessionId}");

            session.Complete();
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Focus session completed: {SessionId} - {Elapsed}s",
                session.Id, session.ElapsedSeconds);
            return MapToDto(session);
        }

        public async Task<List<FocusSessionDto>> GetHistoryAsync(int days = 30, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);

            return await _context.FocusSessions
                .Where(s => !s.IsDeleted && s.StartedAt >= cutoff)
                .OrderByDescending(s => s.StartedAt)
                .AsNoTracking()
                .Select(s => MapToDto(s))
                .ToListAsync(ct);
        }

        private FocusSessionDto MapToDto(FocusSession session)
        {
            return new FocusSessionDto
            {
                Id = session.Id,
                DurationMinutes = session.DurationMinutes,
                ElapsedSeconds = session.ElapsedSeconds,
                Status = session.Status,
                Type = session.Type,
                TaskId = session.TaskId,
                CompletedAt = session.CompletedAt,
                PausedAt = session.PausedAt,
                StartedAt = session.StartedAt,
                RoundsCompleted = session.RoundsCompleted
            };
        }
    }
}
