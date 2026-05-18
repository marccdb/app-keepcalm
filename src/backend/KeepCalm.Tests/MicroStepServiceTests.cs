using FluentAssertions;
using KeepCalm.Data;
using KeepCalm.DTOs;
using KeepCalm.Models.Entities;
using KeepCalm.Services;
using KeepCalm.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KeepCalm.Tests
{
    public class MicroStepServiceTests
    {
        private readonly MongoDbContext _context;
        private readonly List<TaskItem> _tasks;
        private readonly List<MicroStep> _microSteps;
        private readonly MicroStepService _service;

        public MicroStepServiceTests()
        {
            var (mockContext, tasks) = TestHelpers.CreateContext(new List<TaskItem>());
            _context = mockContext.Object;
            _tasks = tasks;

            var (mockMicroSteps, microSteps) = TestHelpers.CreateContext(new List<MicroStep>());
            _microSteps = microSteps;

            var loggerMock = new Mock<ILogger<MicroStepService>>();
            _service = new MicroStepService(_context, loggerMock.Object);
        }

        [Fact]
        public async Task AddMicroStepAsync_AddsMicroStepToTask()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            _tasks.Add(task);

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
            result.OrderIndex.Should().Be(0);
            task.MicroSteps.Should().Contain(result);
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
            _tasks.Add(task);
            _microSteps.Add(existingStep);

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
            _tasks.Add(task);
            _microSteps.Add(microStep);

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
            _tasks.Add(task);
            _microSteps.Add(microStep);

            var dto = new UpdateMicroStepDto { IsCompleted = true };

            // Act
            await _service.UpdateMicroStepAsync(microStep.Id, dto);

            // Assert
            task.Status.Should().Be(TaskItemStatus.Completed);
        }

        [Fact]
        public async Task UpdateMicroStepAsync_DoesNotChangeTaskStatus_WhenNotAllStepsCompleted()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task", Status = TaskItemStatus.InProgress };
            var step1 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 1", IsCompleted = true };
            var step2 = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "Step 2", IsCompleted = false };
            _tasks.Add(task);
            _microSteps.Add(step1);
            _microSteps.Add(step2);

            var dto = new UpdateMicroStepDto { IsCompleted = true };

            // Act
            await _service.UpdateMicroStepAsync(step2.Id, dto);

            // Assert
            task.Status.Should().Be(TaskItemStatus.InProgress);
        }

        [Fact]
        public async Task DeleteMicroStepAsync_SoftDeletesTheMicroStep()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Main Task" };
            var microStep = new MicroStep { Id = Guid.NewGuid(), TaskId = task.Id, Title = "To Delete" };
            _tasks.Add(task);
            _microSteps.Add(microStep);

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

            _tasks.Add(task);
            _microSteps.Add(step1);
            _microSteps.Add(step2);
            _microSteps.Add(step3);

            var orderIndices = new List<int> { 2, 0, 1 };

            // Act
            var result = await _service.ReorderMicroStepsAsync(task.Id, orderIndices);

            // Assert
            result.Should().HaveCount(3);
            result.Should().ContainInOrder(step3, step1, step2);
            step3.OrderIndex.Should().Be(2);
            step1.OrderIndex.Should().Be(0);
            step2.OrderIndex.Should().Be(1);
        }
    }
}
