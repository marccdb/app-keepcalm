using KeepCalm.DTOs;
using KeepCalm.Models.Entities;

namespace KeepCalm.Services
{
    public interface IFocusSessionService
    {
        Task<FocusSessionDto> StartSessionAsync(StartFocusSessionDto dto, CancellationToken ct = default);
        Task<FocusSessionDto> PauseSessionAsync(Guid sessionId, CancellationToken ct = default);
        Task<FocusSessionDto> ResumeSessionAsync(Guid sessionId, CancellationToken ct = default);
        Task<FocusSessionDto> CompleteSessionAsync(Guid sessionId, CancellationToken ct = default);
        Task<List<FocusSessionDto>> GetHistoryAsync(int days = 30, CancellationToken ct = default);
    }
}
