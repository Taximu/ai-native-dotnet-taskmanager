using FluentAssertions;
using TaskManagement.Core.Domain;

namespace TaskManagement.Tests;

public class TaskStatusTransitionValidatorTests
{
    [Theory]
    [InlineData(TaskItemStatus.New, TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.InProgress, TaskItemStatus.Completed)]
    [InlineData(TaskItemStatus.Completed, TaskItemStatus.Archived)]
    public void IsValidTransition_ForwardStep_ReturnsTrue(TaskItemStatus current, TaskItemStatus target)
    {
        TaskStatusTransitionValidator.IsValidTransition(current, target).Should().BeTrue();
    }

    [Theory]
    [InlineData(TaskItemStatus.New)]
    [InlineData(TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.Completed)]
    [InlineData(TaskItemStatus.Archived)]
    public void IsValidTransition_SameStatus_ReturnsTrue(TaskItemStatus status)
    {
        TaskStatusTransitionValidator.IsValidTransition(status, status).Should().BeTrue();
    }

    [Theory]
    [InlineData(TaskItemStatus.Completed, TaskItemStatus.New)]
    [InlineData(TaskItemStatus.Archived, TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.New, TaskItemStatus.Completed)]
    [InlineData(TaskItemStatus.New, TaskItemStatus.Archived)]
    [InlineData(TaskItemStatus.InProgress, TaskItemStatus.New)]
    [InlineData(TaskItemStatus.InProgress, TaskItemStatus.Archived)]
    [InlineData(TaskItemStatus.Archived, TaskItemStatus.New)]
    [InlineData(TaskItemStatus.Archived, TaskItemStatus.Completed)]
    public void IsValidTransition_SkipOrBackward_ReturnsFalse(TaskItemStatus current, TaskItemStatus target)
    {
        TaskStatusTransitionValidator.IsValidTransition(current, target).Should().BeFalse();
    }
}
