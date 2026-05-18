using KeepCalm.DTOs;

namespace KeepCalm.Services
{
    public interface IMicroStepService
    {
        Task<List<MicroStepDto>> GetMicroStepsAsync(Guid taskId, CancellationToken ct = default);
        Task<MicroStepDto> AddMicroStepAsync(Guid taskId, CreateMicroStepDto dto, CancellationToken ct = default);
        Task<MicroStepDto> UpdateMicroStepAsync(Guid microStepId, UpdateMicroStepDto dto, CancellationToken ct = default);
        Task<bool> DeleteMicroStepAsync(Guid microStepId, CancellationToken ct = default);
        Task<List<MicroStepDto>> ReorderMicroStepsAsync(Guid taskId, List<int> orderIndices, CancellationToken ct = default);
        Task<MicroStepDto?> GetMicroStepByIdAsync(Guid microStepId, CancellationToken ct = default);
    }
}
