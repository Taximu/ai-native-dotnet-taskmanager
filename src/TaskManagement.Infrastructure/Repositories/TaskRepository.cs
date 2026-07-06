using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Domain;
using TaskManagement.Core.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository(TaskManagementDbContext dbContext) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(int id) =>
        dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
