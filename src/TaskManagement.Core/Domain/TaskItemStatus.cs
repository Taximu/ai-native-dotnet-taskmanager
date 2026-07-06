namespace TaskManagement.Core.Domain;

// Named TaskItemStatus (not TaskStatus) to avoid colliding with
// System.Threading.Tasks.TaskStatus, which is implicitly in scope
// everywhere async code uses Task/Task<T>.
public enum TaskItemStatus
{
    New,
    InProgress,
    Completed,
    Archived
}
