using KeepCalm.DTOs;

namespace KeepCalm.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksAsync(string? priority = null, string? status = null, CancellationToken ct = default);
        Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken ct = default);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken ct = default);
        Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, CancellationToken ct = default);
        Task<bool> DeleteTaskAsync(Guid id, CancellationToken ct = default);
        Task<List<TaskDto>> ReorderTasksAsync(List<Guid> orderedIds, CancellationToken ct = default);
    }
}
