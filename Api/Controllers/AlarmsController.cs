using Application.AlarmManagement.Commands;
using Application.AlarmManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlarmsController : BaseController
{
    private readonly ISender _mediator;

    public AlarmsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{alarmId:guid}")]
    public async Task<IActionResult> GetById(Guid alarmId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAlarmByIdQuery(alarmId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetActiveAlarmsQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetByTag(Guid tagId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAlarmsByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetByAnchor(Guid anchorId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAlarmsByAnchorIdQuery(anchorId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{alarmId:guid}/acknowledge")]
    public async Task<IActionResult> Acknowledge(Guid alarmId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new AcknowledgeAlarmCommand(alarmId, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "acknowledged" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{alarmId:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid alarmId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ResolveAlarmCommand(alarmId, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "resolved" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{alarmId:guid}/close")]
    public async Task<IActionResult> Close(Guid alarmId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CloseAlarmCommand(alarmId, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "closed" })
            : BadRequest(new { error = result.Error });
    }
}