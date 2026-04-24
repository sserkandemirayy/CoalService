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

    [HttpPost("anchor-config-reported")]
    public async Task<IActionResult> ProcessAnchorConfigReported(
    [FromBody] AnchorConfigReportedPayloadDto payload,
    CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessAnchorConfigReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("ble-config-reported")]
    public async Task<IActionResult> ProcessBleConfigReported(
        [FromBody] BleConfigReportedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessBleConfigReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("uwb-config-reported")]
    public async Task<IActionResult> ProcessUwbConfigReported(
        [FromBody] UwbConfigReportedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessUwbConfigReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("dio-config-reported")]
    public async Task<IActionResult> ProcessDioConfigReported(
        [FromBody] DioConfigReportedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessDioConfigReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("i2c-config-reported")]
    public async Task<IActionResult> ProcessI2cConfigReported(
        [FromBody] I2cConfigReportedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessI2cConfigReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("anchor-config/by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetAnchorConfigEventsByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAnchorConfigEventsByAnchorIdQuery(anchorId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("anchor-config/current/{anchorId:guid}")]
    public async Task<IActionResult> GetCurrentAnchorConfigByAnchorId(Guid anchorId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCurrentAnchorConfigByAnchorIdQuery(anchorId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("ble-config/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetBleConfigEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetBleConfigEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("ble-config/current/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentBleConfigByTagId(Guid tagId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCurrentBleConfigByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("uwb-config/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetUwbConfigEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetUwbConfigEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("uwb-config/current/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentUwbConfigByTagId(Guid tagId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCurrentUwbConfigByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("dio-config/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetDioConfigEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetDioConfigEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("dio-config/current/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentDioConfigsByTagId(Guid tagId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCurrentDioConfigsByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("i2c-config/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetI2cConfigEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetI2cConfigEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("i2c-config/current/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentI2cConfigByTagId(Guid tagId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCurrentI2cConfigByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpPost("ble-advertisement-received")]
    public async Task<IActionResult> ProcessBleAdvertisementReceived(
    [FromBody] BleAdvertisementReceivedPayloadDto payload,
    CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessBleAdvertisementReceivedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("dio-value-reported")]
    public async Task<IActionResult> ProcessDioValueReported(
        [FromBody] DioValueReportedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessDioValueReportedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("i2c-data-received")]
    public async Task<IActionResult> ProcessI2cDataReceived(
        [FromBody] I2cDataReceivedPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ProcessI2cDataReceivedCommand(payload), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value, status = "processed" })
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("ble-advertisements/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetBleAdvertisementEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetBleAdvertisementEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("ble-advertisements/by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetBleAdvertisementEventsByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetBleAdvertisementEventsByAnchorIdQuery(anchorId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("dio-values/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetDioValueEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetDioValueEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("dio-values/current/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentDioValuesByTagId(
        Guid tagId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCurrentDioValuesByTagIdQuery(tagId), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("i2c-data/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetI2cDataEventsByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetI2cDataEventsByTagIdQuery(tagId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}