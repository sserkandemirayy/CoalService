using Api.Models.Alarms;
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

    [Authorize(Policy = "ViewAlarms")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetAlarmsQuery(
                status,
                startDate,
                endDate),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ViewAlarms")]
    [HttpGet("{alarmId:guid}/notes")]
    public async Task<IActionResult> GetNotes(
        Guid alarmId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetAlarmNotesQuery(alarmId),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    [Authorize(Policy = "ManageAlarms")]
    [HttpPost("{alarmId:guid}/notes")]
    public async Task<IActionResult> AddNote(
        Guid alarmId,
        [FromBody] AddAlarmNoteRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new AddAlarmNoteCommand(
                alarmId,
                CurrentUserId,
                request.Note),
            ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ViewAlarms")]
    [HttpGet("{alarmId:guid}")]
    public async Task<IActionResult> GetById(
        Guid alarmId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetAlarmByIdQuery(alarmId),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    [Authorize(Policy = "ViewAlarms")]
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetActiveAlarmsQuery(),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ViewAlarms")]
    [HttpGet("by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetByTag(
        Guid tagId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetAlarmsByTagIdQuery(tagId),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ViewAlarms")]
    [HttpGet("by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetByAnchor(
        Guid anchorId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetAlarmsByAnchorIdQuery(anchorId),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "AcknowledgeAlarms")]
    [HttpPost("{alarmId:guid}/acknowledge")]
    public async Task<IActionResult> Acknowledge(
        Guid alarmId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new AcknowledgeAlarmCommand(
                alarmId,
                CurrentUserId),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "acknowledged" })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageAlarms")]
    [HttpPost("{alarmId:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        Guid alarmId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ResolveAlarmCommand(
                alarmId,
                CurrentUserId),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "resolved" })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageAlarms")]
    [HttpPost("{alarmId:guid}/close")]
    public async Task<IActionResult> Close(
        Guid alarmId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CloseAlarmCommand(
                alarmId,
                CurrentUserId),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "closed" })
            : BadRequest(new { error = result.Error });
    }
}