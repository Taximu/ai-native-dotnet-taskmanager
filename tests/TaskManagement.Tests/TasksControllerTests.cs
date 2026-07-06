using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.Api.Controllers;
using TaskManagement.Core.Dtos;
using TaskManagement.Core.Domain;
using TaskManagement.Core.Services;

namespace TaskManagement.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _service = new();
    private readonly TasksController _sut;

    public TasksControllerTests()
    {
        _sut = new TasksController(_service.Object);
    }

    [Fact]
    public async Task PatchStatus_ValidTransition_ReturnsOkWithUpdatedTask()
    {
        var dto = new TaskDto(1, "Write spec", null, TaskItemStatus.InProgress, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
        _service.Setup(s => s.UpdateStatusAsync(1, "InProgress"))
            .ReturnsAsync(TaskStatusUpdateResult.Success(dto));

        var result = await _sut.PatchStatus(1, new UpdateTaskStatusRequest("InProgress"));

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
    }

    [Fact]
    public async Task PatchStatus_InvalidTransition_ReturnsBadRequest()
    {
        _service.Setup(s => s.UpdateStatusAsync(1, "New"))
            .ReturnsAsync(TaskStatusUpdateResult.InvalidTransition());

        var result = await _sut.PatchStatus(1, new UpdateTaskStatusRequest("New"));

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PatchStatus_TaskNotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.UpdateStatusAsync(99, "InProgress"))
            .ReturnsAsync(TaskStatusUpdateResult.NotFound());

        var result = await _sut.PatchStatus(99, new UpdateTaskStatusRequest("InProgress"));

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PatchStatus_SameStatus_ReturnsOkNoOp()
    {
        var dto = new TaskDto(1, "Write spec", null, TaskItemStatus.InProgress, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
        _service.Setup(s => s.UpdateStatusAsync(1, "InProgress"))
            .ReturnsAsync(TaskStatusUpdateResult.Success(dto));

        var result = await _sut.PatchStatus(1, new UpdateTaskStatusRequest("InProgress"));

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
    }

    [Fact]
    public async Task PatchStatus_InvalidStatusValue_ReturnsBadRequest()
    {
        _service.Setup(s => s.UpdateStatusAsync(1, "NotARealStatus"))
            .ReturnsAsync(TaskStatusUpdateResult.InvalidStatusValue());

        var result = await _sut.PatchStatus(1, new UpdateTaskStatusRequest("NotARealStatus"));

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
