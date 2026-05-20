using KeepCalm.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeepCalm.Data
{
    public interface IMongoDbContext
    {
        DbSet<TaskItem> Tasks { get; }
        DbSet<MicroStep> MicroSteps { get; }
        DbSet<FocusSession> FocusSessions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
