using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Dtos;
using TaskManagement.Core.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] UpdateTaskStatusRequest request)
    {
        var result = await taskService.UpdateStatusAsync(id, request.Status);

        return result.Outcome switch
        {
            TaskStatusUpdateOutcome.Success => Ok(result.Task),
            TaskStatusUpdateOutcome.TaskNotFound => NotFound(),
            TaskStatusUpdateOutcome.InvalidStatusValue => BadRequest("Status value is not a recognized task status."),
            TaskStatusUpdateOutcome.InvalidTransition => BadRequest("The requested status transition is not allowed."),
            _ => throw new InvalidOperationException($"Unhandled outcome: {result.Outcome}")
        };
    }
}
