using TaskManagement.Core.Domain;

namespace TaskManagement.Core.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id);

    Task SaveChangesAsync();
}
