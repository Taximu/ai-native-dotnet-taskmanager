namespace TaskManagement.Core.Services;

public interface ITaskService
{
    Task<TaskStatusUpdateResult> UpdateStatusAsync(int taskId, string requestedStatus);
}
