using KeepCalm.Data;
using KeepCalm.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeepCalm.Tests.Helpers
{
    public static class TestHelpers
    {
        public static TestMongoDbContext CreateContext(List<TaskItem>? tasks = null, List<MicroStep>? microSteps = null)
        {
            var context = TestMongoDbContext.Create("TestDb");
            
            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    context.Tasks.Add(task);
                }
            }
            
            if (microSteps != null)
            {
                foreach (var step in microSteps)
                {
                    context.MicroSteps.Add(step);
                }
            }
            
            return context;
        }

        public static async Task<TestMongoDbContext> CreateContextWithTasks(List<TaskItem> tasks)
        {
            var context = CreateContext(tasks: tasks);
            await context.SaveChangesAsync();
            return context;
        }

        public static async Task<TestMongoDbContext> CreateContextWithMicroSteps(List<MicroStep> microSteps)
        {
            var context = CreateContext(microSteps: microSteps);
            await context.SaveChangesAsync();
            return context;
        }

        public static async Task<TestMongoDbContext> CreateContextWithTasksAndMicroSteps(List<TaskItem> tasks, List<MicroStep> microSteps)
        {
            var context = CreateContext(tasks, microSteps);
            await context.SaveChangesAsync();
            return context;
        }

        public static void ClearData(TestMongoDbContext context)
        {
            context.Tasks.RemoveRange(context.Tasks);
            context.MicroSteps.RemoveRange(context.MicroSteps);
            context.FocusSessions.RemoveRange(context.FocusSessions);
        }
    }
}
