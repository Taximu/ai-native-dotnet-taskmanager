using TaskManagement.Core.Domain;

namespace TaskManagement.Core.Dtos;

public record TaskDto(
    int Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc)
{
    public static TaskDto FromDomain(TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.CreatedAtUtc,
        task.UpdatedAtUtc);
}
