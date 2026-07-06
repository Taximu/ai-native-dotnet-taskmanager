using TaskManagement.Core.Domain;
using TaskManagement.Core.Dtos;
using TaskManagement.Core.Repositories;

namespace TaskManagement.Core.Services;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<TaskStatusUpdateResult> UpdateStatusAsync(int taskId, string requestedStatus)
    {
        if (!Enum.TryParse<TaskItemStatus>(requestedStatus, ignoreCase: true, out var targetStatus)
            || !Enum.IsDefined(targetStatus))
        {
            return TaskStatusUpdateResult.InvalidStatusValue();
        }

        var task = await taskRepository.GetByIdAsync(taskId);
        if (task is null)
        {
            return TaskStatusUpdateResult.NotFound();
        }

        if (!TaskStatusTransitionValidator.IsValidTransition(task.Status, targetStatus))
        {
            return TaskStatusUpdateResult.InvalidTransition();
        }

        if (task.Status != targetStatus)
        {
            task.Status = targetStatus;
            task.UpdatedAtUtc = DateTimeOffset.UtcNow;
            await taskRepository.SaveChangesAsync();
        }

        return TaskStatusUpdateResult.Success(TaskDto.FromDomain(task));
    }
}
