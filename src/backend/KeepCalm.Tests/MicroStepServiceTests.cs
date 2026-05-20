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
    public class MicroStepServiceTests
    {
        private readonly TestMongoDbContext _context;
        private readonly MicroStepService _service;

        public MicroStepServiceTests()
        {
            var dbName = $"MicroStepServiceTestDb_{Guid.NewGuid():N}";
            _context = TestMongoDbContext.Create(dbName);
            var loggerMock = new Mock<ILogger<MicroStepService>>();
            _service = new MicroStepService(_context, loggerMock.Object);
        }

        [Fact]
        public async Task AddMicroStepAsync_AddsMicroStepToTask()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var dto = new CreateMicroStepDto
            {
                Title = "New Micro Step",
                Description = "Step Description"
            };

            // Act
            var result = await _service.AddMicroStepAsync(task.Id, dto);

            // Assert
            result.Title.Should().Be("New Micro Step");
            result.Description.Should().Be("Step Description");
            result.TaskId.Should().Be(task.Id);
            result.IsCompleted.Should().BeFalse();
            result.OrderIndex.Should().Be(1);
        }

        [Fact]
        public async Task AddMicroStepAsync_ThrowsKeyNotFoundException_ForNonExistentTask()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var dto = new CreateMicroStepDto { Title = "Step" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.AddMicroStepAsync(nonExistentId, dto));
        }

        [Fact]
        public async Task AddMicroStepAsync_AssignsCorrectOrderIndex()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            var existingStep = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 1", OrderIndex = 3 };
            _context.Tasks.Add(task);
            _context.MicroSteps.Add(existingStep);
            await _context.SaveChangesAsync();

            var dto = new CreateMicroStepDto { Title = "New Step" };

            // Act
            var result = await _service.AddMicroStepAsync(task.Id, dto);

            // Assert
            result.OrderIndex.Should().Be(4);
        }

        [Fact]
        public async Task UpdateMicroStepAsync_UpdatesMicroStepProperties()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            var microStep = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Original", IsCompleted = false };
            _context.Tasks.Add(task);
            _context.MicroSteps.Add(microStep);
            await _context.SaveChangesAsync();

            var dto = new UpdateMicroStepDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                IsCompleted = true
            };

            // Act
            var result = await _service.UpdateMicroStepAsync(microStep.Id, dto);

            // Assert
            result.Title.Should().Be("Updated Title");
            result.Description.Should().Be("Updated Description");
            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateMicroStepAsync_ThrowsKeyNotFoundException_ForNonExistentMicroStep()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var dto = new UpdateMicroStepDto { Title = "Updated" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.UpdateMicroStepAsync(nonExistentId, dto));
        }

        [Fact]
        public async Task UpdateMicroStepAsync_UpdatesTaskStatus_WhenAllStepsCompleted()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task", Status = TaskItemStatus.Pending };
            var microStep = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 1", IsCompleted = false };
            _context.Tasks.Add(task);
            _context.MicroSteps.Add(microStep);
            await _context.SaveChangesAsync();

            var dto = new UpdateMicroStepDto { IsCompleted = true };

            // Act
            await _service.UpdateMicroStepAsync(microStep.Id, dto);

            // Assert
            var updatedTask = await _context.Tasks.FindAsync(task.Id);
            updatedTask!.Status.Should().Be(TaskItemStatus.Completed);
        }

        [Fact]
        public async Task UpdateMicroStepAsync_DoesNotChangeTaskStatus_WhenNotAllStepsCompleted()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task", Status = TaskItemStatus.InProgress };
            var step1 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 1", IsCompleted = true };
            var step2 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 2", IsCompleted = false };
            _context.Tasks.Add(task);
            _context.MicroSteps.Add(step1);
            _context.MicroSteps.Add(step2);
            await _context.SaveChangesAsync();

            var dto = new UpdateMicroStepDto { IsCompleted = true };

            // Act
            await _service.UpdateMicroStepAsync(step2.Id, dto);

            // Assert
            var updatedTask = await _context.Tasks.FindAsync(task.Id);
            updatedTask!.Status.Should().Be(TaskItemStatus.Completed);
        }

        [Fact]
        public async Task DeleteMicroStepAsync_SoftDeletesTheMicroStep()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            var microStep = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "To Delete" };
            _context.Tasks.Add(task);
            _context.MicroSteps.Add(microStep);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteMicroStepAsync(microStep.Id);

            // Assert
            result.Should().BeTrue();
            microStep.IsDeleted.Should().BeTrue();
            microStep.DeletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteMicroStepAsync_ReturnsFalse_ForNonExistentMicroStep()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteMicroStepAsync(nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ReorderMicroStepsAsync_UpdatesOrderIndexForAllMicroSteps()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            var step1 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 1", OrderIndex = 0 };
            var step2 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 2", OrderIndex = 1 };
            var step3 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 3", OrderIndex = 2 };

            _context.Tasks.Add(task);
            _context.MicroSteps.Add(step1);
            _context.MicroSteps.Add(step2);
            _context.MicroSteps.Add(step3);
            await _context.SaveChangesAsync();

            var orderIndices = new List<int> { 2, 0, 1 };

            // Act
            var result = await _service.ReorderMicroStepsAsync(task.Id, orderIndices);

            // Assert
            result.Should().HaveCount(3);
            result[0].Id.Should().Be(step2.Id);
            result[0].OrderIndex.Should().Be(0);
            result[1].Id.Should().Be(step3.Id);
            result[1].OrderIndex.Should().Be(1);
            result[2].Id.Should().Be(step1.Id);
            result[2].OrderIndex.Should().Be(2);
        }
    }
}
