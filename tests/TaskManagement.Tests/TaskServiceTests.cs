using FluentAssertions;
using Moq;
using TaskManagement.Core.Domain;
using TaskManagement.Core.Repositories;
using TaskManagement.Core.Services;

namespace TaskManagement.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repository = new();
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _sut = new TaskService(_repository.Object);
    }

    [Fact]
    public async Task UpdateStatusAsync_ValidTransition_ReturnsSuccessWithUpdatedTask()
    {
        var task = new TaskItem { Id = 1, Title = "Write spec", Status = TaskItemStatus.New };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _sut.UpdateStatusAsync(1, "InProgress");

        result.Outcome.Should().Be(TaskStatusUpdateOutcome.Success);
        result.Task!.Status.Should().Be(TaskItemStatus.InProgress);
        task.Status.Should().Be(TaskItemStatus.InProgress);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_InvalidTransition_ReturnsInvalidTransitionAndDoesNotSave()
    {
        var task = new TaskItem { Id = 1, Title = "Write spec", Status = TaskItemStatus.Completed };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _sut.UpdateStatusAsync(1, "New");

        result.Outcome.Should().Be(TaskStatusUpdateOutcome.InvalidTransition);
        result.Task.Should().BeNull();
        task.Status.Should().Be(TaskItemStatus.Completed);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_TaskNotFound_ReturnsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem?)null);

        var result = await _sut.UpdateStatusAsync(99, "InProgress");

        result.Outcome.Should().Be(TaskStatusUpdateOutcome.TaskNotFound);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_SameStatus_ReturnsSuccessNoOpWithoutSaving()
    {
        var task = new TaskItem { Id = 1, Title = "Write spec", Status = TaskItemStatus.InProgress };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _sut.UpdateStatusAsync(1, "InProgress");

        result.Outcome.Should().Be(TaskStatusUpdateOutcome.Success);
        result.Task!.Status.Should().Be(TaskItemStatus.InProgress);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_StatusValueNotInEnum_ReturnsInvalidStatusValue()
    {
        var result = await _sut.UpdateStatusAsync(1, "NotARealStatus");

        result.Outcome.Should().Be(TaskStatusUpdateOutcome.InvalidStatusValue);
        _repository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }
}
