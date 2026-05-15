using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using KeepCalm.Models.Entities;

namespace KeepCalm.Data
{
    public class MongoDbContext : DbContext
    {
        public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<MicroStep> MicroSteps => Set<MicroStep>();
        public DbSet<FocusSession> FocusSessions => Set<FocusSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MongoDB stores documents as-is; collection names use lowercase
            modelBuilder.Entity<TaskItem>().ToCollection("tasks");
            modelBuilder.Entity<MicroStep>().ToCollection("micro_steps");
            modelBuilder.Entity<FocusSession>().ToCollection("focus_sessions");

            // Indexes
            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.Priority);
            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.Status);
            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.IsDeleted);

            modelBuilder.Entity<MicroStep>()
                .HasIndex(ms => ms.TaskId);
            modelBuilder.Entity<FocusSession>()
                .HasIndex(fs => fs.Status);
            modelBuilder.Entity<FocusSession>()
                .HasIndex(fs => fs.StartedAt);
            modelBuilder.Entity<FocusSession>()
                .HasIndex(fs => fs.IsDeleted);
        }
    }
}
