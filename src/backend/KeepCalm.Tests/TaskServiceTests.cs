using FluentAssertions;
using KeepCalm.Data;
using KeepCalm.DTOs;
using KeepCalm.Models.Entities;
using KeepCalm.Services;
using KeepCalm.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KeepCalm.Tests
{
    public class TaskServiceTests
    {
        private readonly TestMongoDbContext _context;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            var dbName = $"TaskServiceTestDb_{Guid.NewGuid():N}";
            _context = TestMongoDbContext.Create(dbName);
            var loggerMock = new Mock<ILogger<TaskService>>();
            _service = new TaskService(_context, loggerMock.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_CreatesTaskWithCorrectProperties()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                Priority = TaskPriority.Urgent
            };

            // Act
            var result = await _service.CreateTaskAsync(dto);

            // Assert
            result.Title.Should().Be("Test Task");
            result.Description.Should().Be("Test Description");
            result.Priority.Should().Be(TaskPriority.Urgent);
            result.Status.Should().Be(TaskItemStatus.Pending);
            result.OrderIndex.Should().Be(1);
            result.MicroSteps.Should().BeEmpty();
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task CreateTaskAsync_TrimsTitleWhitespace()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "  Trimmed Task  ",
                Description = "  Trimmed Description  "
            };

            // Act
            var result = await _service.CreateTaskAsync(dto);

            // Assert
            result.Title.Should().Be("Trimmed Task");
            result.Description.Should().Be("Trimmed Description");
        }

        [Fact]
        public async Task CreateTaskAsync_AssignsCorrectOrderIndex()
        {
            // Arrange
            var existingTask = new TaskItem { Id = Guid.NewGuid(), Title = "Existing", OrderIndex = 5 };
            _context.Tasks.Add(existingTask);
            await _context.SaveChangesAsync();

            var dto = new CreateTaskDto { Title = "New Task" };

            // Act
            var result = await _service.CreateTaskAsync(dto);

            // Assert
            result.OrderIndex.Should().Be(6);
        }

        [Fact]
        public async Task GetTasksAsync_ReturnsNonDeletedTasks()
        {
            // Arrange
            var task1 = new TaskItem { Id = Guid.NewGuid(), Title = "First", OrderIndex = 1 };
            var task2 = new TaskItem { Id = Guid.NewGuid(), Title = "Second", OrderIndex = 2 };
            var deletedTask = new TaskItem { Id = Guid.NewGuid(), Title = "Deleted", OrderIndex = 3 };
            deletedTask.SoftDelete();

            _context.Tasks.Add(task1);
            _context.Tasks.Add(task2);
            _context.Tasks.Add(deletedTask);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTasksAsync();

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("First");
            result[1].Title.Should().Be("Second");
        }

        [Fact]
        public async Task GetTasksAsync_FiltersByPriority()
        {
            // Arrange
            _context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Title = "Urgent Task", Priority = TaskPriority.Urgent });
            _context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Title = "Normal Task", Priority = TaskPriority.Normal });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTasksAsync(priority: "Urgent");

            // Assert
            result.Should().HaveCount(1);
            result[0].Priority.Should().Be(TaskPriority.Urgent);
        }

        [Fact]
        public async Task GetTasksAsync_FiltersByStatus()
        {
            // Arrange
            _context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Title = "Pending", Status = TaskItemStatus.Pending });
            _context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Title = "Completed", Status = TaskItemStatus.Completed });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTasksAsync(status: "Pending");

            // Assert
            result.Should().HaveCount(1);
            result[0].Status.Should().Be(TaskItemStatus.Pending);
        }

        [Fact]
        public async Task UpdateTaskAsync_UpdatesTaskProperties()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Original" };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var dto = new UpdateTaskDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Priority = TaskPriority.Urgent,
                Status = TaskItemStatus.InProgress
            };

            // Act
            var result = await _service.UpdateTaskAsync(task.Id, dto);

            // Assert
            result.Title.Should().Be("Updated Title");
            result.Description.Should().Be("Updated Description");
            result.Priority.Should().Be(TaskPriority.Urgent);
            result.Status.Should().Be(TaskItemStatus.InProgress);
        }

        [Fact]
        public async Task UpdateTaskAsync_ThrowsKeyNotFoundException_ForNonExistentTask()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var dto = new UpdateTaskDto { Title = "Updated" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.UpdateTaskAsync(nonExistentId, dto));
        }

        [Fact]
        public async Task DeleteTaskAsync_SoftDeletesTheTask()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "To Delete" };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteTaskAsync(task.Id);

            // Assert
            result.Should().BeTrue();
            task.IsDeleted.Should().BeTrue();
            task.DeletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteTaskAsync_ReturnsFalse_ForNonExistentTask()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteTaskAsync(nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ReorderTasksAsync_UpdatesOrderIndexForAllTasks()
        {
            // Arrange
            var task1 = new TaskItem { Id = Guid.NewGuid(), Title = "First", OrderIndex = 0 };
            var task2 = new TaskItem { Id = Guid.NewGuid(), Title = "Second", OrderIndex = 1 };
            var task3 = new TaskItem { Id = Guid.NewGuid(), Title = "Third", OrderIndex = 2 };

            _context.Tasks.Add(task1);
            _context.Tasks.Add(task2);
            _context.Tasks.Add(task3);
            await _context.SaveChangesAsync();

            var orderedIds = new List<Guid> { task3.Id, task1.Id, task2.Id };

            // Act
            var result = await _service.ReorderTasksAsync(orderedIds);

            // Assert
            result.Should().HaveCount(3);
            result[0].Title.Should().Be("Third");
            result[0].OrderIndex.Should().Be(0);
            result[1].Title.Should().Be("First");
            result[1].OrderIndex.Should().Be(1);
            result[2].Title.Should().Be("Second");
            result[2].OrderIndex.Should().Be(2);
        }
    }
}
