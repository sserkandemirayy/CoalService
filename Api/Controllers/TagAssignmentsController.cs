using Application.DeviceManagment.Commands;
using Application.DeviceManagment.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagAssignmentsController : BaseController
{
    private readonly ISender _mediator;

    public TagAssignmentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetByTagId(Guid tagId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTagAssignmentsByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTagAssignmentsByUserIdQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Assign(
        [FromBody] AssignTagRequest request,
        CancellationToken ct)
    {
        var command = new AssignTagToUserCommand(
            request.TagId,
            request.UserId,
            request.IsPrimary,
            CurrentUserId);

        var result = await _mediator.Send(command, ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "assigned" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("unassign")]
    public async Task<IActionResult> Unassign(
        [FromBody] UnassignTagRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UnassignTagCommand(request.TagId, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "unassigned" })
            : BadRequest(new { error = result.Error });
    }

    public sealed record AssignTagRequest(
        Guid TagId,
        Guid UserId,
        bool IsPrimary
    );

    public sealed record UnassignTagRequest(Guid TagId);
}