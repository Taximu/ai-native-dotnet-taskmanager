# Spec-Driven Development — Task Management Service v1.0

This spec is written so an AI coding agent can implement each story without
further clarification. Each story has acceptance criteria, edge cases, a
test-first approach, and an ordered implementation plan.

---

## Story 1: Update Task Status (Simple)

**As a** user
**I want** to change a task's status
**So that** I can track its progress through the workflow

### Acceptance Criteria
- Given a task exists with status `New`
- When `PATCH /tasks/{id}/status` is called with `{ "status": "InProgress" }`
- Then the task's status is updated and `200 OK` is returned with the updated task
- Valid transitions: `New → InProgress → Completed → Archived`
- Any other transition (e.g. `Completed → New`) returns `400 Bad Request`

### Edge Cases
- Task ID does not exist → `404 Not Found`
- Status value not in the enum → `400 Bad Request`
- Same status as current → `200 OK`, no-op (idempotent)

### Test Approach (write these first)
```csharp
[Fact]
public async Task UpdateStatus_ValidTransition_ReturnsOkWithUpdatedTask() { }

[Fact]
public async Task UpdateStatus_InvalidTransition_ReturnsBadRequest() { }

[Fact]
public async Task UpdateStatus_TaskNotFound_ReturnsNotFound() { }

[Fact]
public async Task UpdateStatus_SameStatus_ReturnsOkNoOp() { }
```

### Implementation Plan
1. `TaskStatus.cs` enum (New, InProgress, Completed, Archived)
2. `TaskStatusTransitionValidator.cs` — pure function, easy to unit test in isolation
3. `TaskService.UpdateStatusAsync()` — calls validator, then repository
4. `TaskController.PatchStatus()` — endpoint, maps errors to status codes
5. Tests for validator (pure logic) + tests for service (with mocked repo) + tests for controller

---

## Story 2: List Tasks with Filtering & Pagination (Medium)

**As a** user
**I want** to filter and page through my tasks
**So that** I can find relevant tasks without loading everything

### Acceptance Criteria
- Given tasks exist with varying priority and status
- When `GET /tasks?priority=High&status=InProgress&page=1&pageSize=10` is called
- Then only matching tasks are returned, paginated, with `200 OK`
- Response includes `{ items, totalCount, page, pageSize }`
- All filter parameters are optional — omitting them returns all tasks (paginated)
- Default `page=1`, `pageSize=20` if not specified

### Edge Cases
- `page=0` or negative → treat as `page=1`
- `pageSize=0` → `400 Bad Request`
- `pageSize` > 100 → cap at 100
- `page` beyond available data → `200 OK` with empty `items`, correct `totalCount`
- Invalid enum value for `priority` or `status` → `400 Bad Request`

### Test Approach
```csharp
[Fact]
public async Task ListTasks_NoFilters_ReturnsAllPaginated() { }

[Fact]
public async Task ListTasks_FilterByPriorityAndStatus_ReturnsMatchingOnly() { }

[Fact]
public async Task ListTasks_PageBeyondData_ReturnsEmptyItemsCorrectTotalCount() { }

[Fact]
public async Task ListTasks_PageSizeExceedsMax_CapsAt100() { }

[Fact]
public async Task ListTasks_InvalidPriorityValue_ReturnsBadRequest() { }
```

### Implementation Plan
1. `TaskListQuery.cs` — DTO for query parameters
2. `PagedResult<T>.cs` — generic pagination wrapper
3. `TaskRepository.GetFilteredAsync()` — EF Core query with `.AsNoTracking()`, applies filters conditionally
4. `TaskService.ListAsync()` — validation (pageSize cap, page floor) + delegates to repository
5. `TaskController.GetTasks()` — endpoint, binds query params
6. Tests: repository (in-memory SQLite), service (validation logic), controller (parameter binding)

**Pitfall to watch for:** building the EF Core query with string concatenation
or multiple round trips causes N+1 issues — filters should compose into a
single `IQueryable` before materializing.

---

## Story 3: Assign Task with Notification (Complex — includes simulated MCP call)

**As a** user
**I want** to assign a task to someone and have them notified
**So that** the assignee knows they have new work

### Acceptance Criteria
- Given a task exists and an assignee email is provided
- When `POST /tasks/{id}/assign` is called with `{ "assigneeEmail": "..." }`
- Then the task's assignee is updated, a notification is dispatched (simulated MCP call), and `200 OK` is returned
- If the notification dispatch fails, the assignment still succeeds — notification failure must not roll back the assignment (log it instead)

### Edge Cases
- Task not found → `404 Not Found`
- Invalid email format → `400 Bad Request`
- Reassigning an already-assigned task → allowed, overwrites, notification sent to new assignee only
- Notification service is simulated as unreachable (test this explicitly) → assignment still commits

### Design Note (required before implementation — this is a "Complex" story)
The notification dispatch is simulated via an `INotificationGateway` interface
with a fake implementation that logs instead of calling a real service. This
mirrors how a real MCP connection would be swapped in later — the interface
boundary is the important part to get right, not the fake implementation.

### Test Approach
```csharp
[Fact]
public async Task AssignTask_ValidRequest_UpdatesAssigneeAndDispatchesNotification() { }

[Fact]
public async Task AssignTask_NotificationGatewayFails_AssignmentStillSucceeds() { }

[Fact]
public async Task AssignTask_InvalidEmail_ReturnsBadRequest() { }

[Fact]
public async Task AssignTask_TaskNotFound_ReturnsNotFound() { }
```

### Implementation Plan
1. `INotificationGateway.cs` — interface (`Task NotifyAsync(string email, string message)`)
2. `SimulatedNotificationGateway.cs` — logs to console/ILogger instead of calling a real service; document in `docs/MCP_CONNECTIONS.md` how this maps to a real ADO/notification MCP call
3. `TaskService.AssignAsync()` — updates assignee, calls gateway, catches gateway exceptions without failing the whole operation
4. `TaskController.AssignTask()` — endpoint
5. Tests: gateway failure simulation (throw from fake gateway, assert assignment still committed), email validation, full happy path

---

## General Notes for Agents

- Write the test file first, get it to fail for the right reason, then implement.
- Every story above should live on its own feature branch: `feature/task-status`, `feature/task-list-filter`, `feature/task-assign`.
- Do not implement all three in one branch — the point of this repo is to
  show each one going through the full agentic loop independently.
