namespace TaskManagement.Core.Domain;

// Pure function: no I/O, no dependencies — kept in isolation so the
// transition rules can be unit tested without a repository or service.
public static class TaskStatusTransitionValidator
{
    private static readonly Dictionary<TaskItemStatus, TaskItemStatus> AllowedForwardTransition = new()
    {
        [TaskItemStatus.New] = TaskItemStatus.InProgress,
        [TaskItemStatus.InProgress] = TaskItemStatus.Completed,
        [TaskItemStatus.Completed] = TaskItemStatus.Archived
    };

    public static bool IsValidTransition(TaskItemStatus currentStatus, TaskItemStatus targetStatus)
    {
        if (currentStatus == targetStatus)
        {
            return true;
        }

        return AllowedForwardTransition.TryGetValue(currentStatus, out var next) && next == targetStatus;
    }
}
