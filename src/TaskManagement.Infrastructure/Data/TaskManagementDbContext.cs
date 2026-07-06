using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Domain;

namespace TaskManagement.Infrastructure.Data;

public class TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(t => t.Title).IsRequired();
            entity.Property(t => t.Status).HasConversion<string>();
        });
    }
}
