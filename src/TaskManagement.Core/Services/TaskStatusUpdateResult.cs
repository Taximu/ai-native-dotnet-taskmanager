using TaskManagement.Core.Dtos;

namespace TaskManagement.Core.Services;

public enum TaskStatusUpdateOutcome
{
    Success,
    TaskNotFound,
    InvalidStatusValue,
    InvalidTransition
}

public record TaskStatusUpdateResult(TaskStatusUpdateOutcome Outcome, TaskDto? Task)
{
    public static TaskStatusUpdateResult Success(TaskDto task) => new(TaskStatusUpdateOutcome.Success, task);

    public static TaskStatusUpdateResult NotFound() => new(TaskStatusUpdateOutcome.TaskNotFound, null);

    public static TaskStatusUpdateResult InvalidStatusValue() => new(TaskStatusUpdateOutcome.InvalidStatusValue, null);

    public static TaskStatusUpdateResult InvalidTransition() => new(TaskStatusUpdateOutcome.InvalidTransition, null);
}
