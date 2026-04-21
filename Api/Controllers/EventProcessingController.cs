using Application.EventProcessing.Commands;
using Application.EventProcessing.Queries;
using Application.DTOs.EventProcessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventProcessingController : BaseController
{
    private readonly ISender _mediator;

    public EventProcessingController(ISender mediator)
    {
        _mediator = mediator;
    }

    // =========================================================
    // PROCESS EVENT COMMANDS
    // =========================================================

    [HttpPost("location-calculated")]
    public async Task<IActionResult> ProcessLocationCalculated(
        [FromBody] LocationCalculatedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessLocationCalculatedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("emergency-button-pressed")]
    public async Task<IActionResult> ProcessEmergencyButtonPressed(
        [FromBody] EmergencyButtonPressedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessEmergencyButtonPressedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("proximity-alert-raised")]
    public async Task<IActionResult> ProcessProximityAlertRaised(
        [FromBody] ProximityAlertRaisedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessProximityAlertRaisedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("imu-event-detected")]
    public async Task<IActionResult> ProcessImuEventDetected(
        [FromBody] ImuEventDetectedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessImuEventDetectedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("battery-level-reported")]
    public async Task<IActionResult> ProcessBatteryLevelReported(
        [FromBody] BatteryLevelReportedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessBatteryLevelReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("anchor-heartbeat-received")]
    public async Task<IActionResult> ProcessAnchorHeartbeatReceived(
        [FromBody] AnchorHeartbeatReceivedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessAnchorHeartbeatReceivedCommand(payload), ct);

        

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("anchor-status-changed")]
    public async Task<IActionResult> ProcessAnchorStatusChanged(
        [FromBody] AnchorStatusChangedPayloadDto payload,
        CancellationToken ct)
    {

        var result = await _mediator.Send(new ProcessAnchorStatusChangedCommand(payload), ct);

        

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    // =========================================================
    // EVENT QUERIES
    // =========================================================

    [HttpGet("raw/{id:guid}")]
    public async Task<IActionResult> GetRawEventById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRawEventByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("raw")]
    public async Task<IActionResult> GetRawEvents(
        [FromQuery] string? eventType,
        [FromQuery] string? tagExternalId,
        [FromQuery] string? anchorExternalId,
        [FromQuery] string? processingStatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetRawEventsQuery(
                eventType,
                tagExternalId,
                anchorExternalId,
                processingStatus,
                page,
                pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("locations/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetLocationEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetLocationEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("battery/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetBatteryEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetBatteryEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("imu/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetImuEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetImuEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("proximity/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetProximityEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetProximityEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("emergency/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetEmergencyEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetEmergencyEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("anchor-heartbeats/by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetAnchorHeartbeatEventsByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAnchorHeartbeatEventsByAnchorIdQuery(anchorId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("anchor-status/by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetAnchorStatusEventsByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAnchorStatusEventsByAnchorIdQuery(anchorId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /////////////-------/////////////

    [HttpPost("anchor-health-reported")]
    public async Task<IActionResult> ProcessAnchorHealthReported(
    [FromBody] AnchorHealthReportedPayloadDto payload,
    CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessAnchorHealthReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("tag-data-received")]
    public async Task<IActionResult> ProcessTagDataReceived(
        [FromBody] TagDataReceivedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessTagDataReceivedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("uwb-ranging-completed")]
    public async Task<IActionResult> ProcessUwbRangingCompleted(
        [FromBody] UwbRangingCompletedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessUwbRangingCompletedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("uwb-tag-to-tag-ranging-completed")]
    public async Task<IActionResult> ProcessUwbTagToTagRangingCompleted(
        [FromBody] UwbTagToTagRangingCompletedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessUwbTagToTagRangingCompletedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("anchor-health/by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetAnchorHealthEventsByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAnchorHealthEventsByAnchorIdQuery(anchorId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("tag-data/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetTagDataEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetTagDataEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("uwb-ranging/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetUwbRangingEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetUwbRangingEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("uwb-ranging/by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetUwbRangingEventsByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetUwbRangingEventsByAnchorIdQuery(anchorId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("uwb-tag-to-tag-ranging/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetUwbTagToTagRangingEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetUwbTagToTagRangingEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}