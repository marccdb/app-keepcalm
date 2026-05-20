using KeepCalm.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeepCalm.Data
{
    public class TestMongoDbContext : DbContext, IMongoDbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<MicroStep> MicroSteps => Set<MicroStep>();
        public DbSet<FocusSession> FocusSessions => Set<FocusSession>();

        public TestMongoDbContext()
        {
        }

        public static TestMongoDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<TestMongoDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new TestMongoDbContext(options);
        }

        protected TestMongoDbContext(DbContextOptions<TestMongoDbContext> options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
