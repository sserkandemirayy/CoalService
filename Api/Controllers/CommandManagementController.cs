using Application.CommandManagement.Commands;
using Application.CommandManagement.Queries;
using Application.DTOs.CommandManagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Security;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommandManagementController : BaseController
{
    private readonly ISender _mediator;

    public CommandManagementController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("request-config")]
    public async Task<IActionResult> CreateRequestConfig(
        [FromBody] RequestConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRequestConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("reset-device")]
    public async Task<IActionResult> CreateResetDevice(
        [FromBody] ResetDeviceCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateResetDeviceCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-anchor-config")]
    public async Task<IActionResult> CreateSetAnchorConfig(
        [FromBody] SetAnchorConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetAnchorConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-ble-config")]
    public async Task<IActionResult> CreateSetBleConfig(
        [FromBody] SetBleConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetBleConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-dio-config")]
    public async Task<IActionResult> CreateSetDioConfig(
        [FromBody] SetDioConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetDioConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-dio-value")]
    public async Task<IActionResult> CreateSetDioValue(
        [FromBody] SetDioValueCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetDioValueCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-i2c-config")]
    public async Task<IActionResult> CreateSetI2cConfig(
        [FromBody] SetI2cConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetI2cConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-proximity-config")]
    public async Task<IActionResult> CreateSetProximityConfig(
        [FromBody] SetProximityConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetProximityConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-tag-alert")]
    public async Task<IActionResult> CreateSetTagAlert(
        [FromBody] SetTagAlertCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetTagAlertCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("set-uwb-config")]
    public async Task<IActionResult> CreateSetUwbConfig(
        [FromBody] SetUwbConfigCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSetUwbConfigCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("write-i2c-data")]
    public async Task<IActionResult> CreateWriteI2cData(
        [FromBody] WriteI2cDataCommandPayloadDto payload,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateWriteI2cDataCommand(payload, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/queue")]
    public async Task<IActionResult> QueueCommand(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new QueueCommandRequestCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { status = "queued" }) : BadRequest(new { error = result.Error });
    }

    public sealed record CancelCommandBody(string? Reason);

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelCommand(Guid id, [FromBody] CancelCommandBody body, CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelCommandRequestCommand(id, CurrentUserId, body?.Reason), ct);
        return result.IsSuccess ? Ok(new { status = "cancelled" }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/retry")]
    public async Task<IActionResult> RetryCommand(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new RetryCommandRequestCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { status = "pending" }) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCommandRequestByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? commandType,
        [FromQuery] string? status,
        [FromQuery] string? targetType,
        [FromQuery] Guid? tagId,
        [FromQuery] Guid? anchorId,
        [FromQuery] Guid? requestedByUserId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetCommandRequestsQuery(
                commandType,
                status,
                targetType,
                tagId,
                anchorId,
                requestedByUserId,
                page,
                pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetByTagId(
        Guid tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCommandRequestsByTagIdQuery(tagId, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("by-anchor/{anchorId:guid}")]
    public async Task<IActionResult> GetByAnchorId(
        Guid anchorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCommandRequestsByAnchorIdQuery(anchorId, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCommandStatusHistoryByCommandIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [AllowAnonymous]
    [RequireIntegrationApiKey]
    [HttpGet("integration/outbox/pending")]
    public async Task<IActionResult> GetPendingOutboxMessages(
    [FromQuery] int take = 50,
    CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPendingOutboxMessagesQuery(take), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [AllowAnonymous]
    [RequireIntegrationApiKey]
    [HttpPost("integration/outbox/{id:guid}/mark-dispatched")]
    public async Task<IActionResult> MarkOutboxDispatched(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new MarkOutboxMessageDispatchedCommand(id), ct);
        return result.IsSuccess ? Ok(new { status = "dispatched" }) : BadRequest(new { error = result.Error });
    }

    [AllowAnonymous]
    [RequireIntegrationApiKey]
    [HttpPost("integration/outbox/{id:guid}/mark-failed")]
    public async Task<IActionResult> MarkOutboxFailed(
        Guid id,
        [FromBody] MarkOutboxFailedDto body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new MarkOutboxMessageFailedCommand(id, body?.FailureReason), ct);
        return result.IsSuccess ? Ok(new { status = "failed" }) : BadRequest(new { error = result.Error });
    }

    [AllowAnonymous]
    [RequireIntegrationApiKey]
    [HttpPost("integration/commands/{id:guid}/mark-sent")]
    public async Task<IActionResult> MarkCommandSentByIntegration(
        Guid id,
        [FromBody] MarkCommandSentByIntegrationDto body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkCommandSentByIntegrationCommand(id, body?.ExternalCorrelationId, body?.Note), ct);

        return result.IsSuccess ? Ok(new { status = "sent" }) : BadRequest(new { error = result.Error });
    }

    [AllowAnonymous]
    [RequireIntegrationApiKey]
    [HttpPost("integration/commands/{id:guid}/mark-succeeded")]
    public async Task<IActionResult> MarkCommandSucceededByIntegration(
        Guid id,
        [FromBody] MarkCommandSucceededByIntegrationDto body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkCommandSucceededByIntegrationCommand(
                id,
                body?.ExternalCorrelationId,
                body?.ResponseJson,
                body?.Note), ct);

        return result.IsSuccess ? Ok(new { status = "succeeded" }) : BadRequest(new { error = result.Error });
    }

    [AllowAnonymous]
    [RequireIntegrationApiKey]
    [HttpPost("integration/commands/{id:guid}/mark-failed")]
    public async Task<IActionResult> MarkCommandFailedByIntegration(
        Guid id,
        [FromBody] MarkCommandFailedByIntegrationDto body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkCommandFailedByIntegrationCommand(
                id,
                body?.ExternalCorrelationId,
                body?.FailureReason,
                body?.ResponseJson,
                body?.Note), ct);

        return result.IsSuccess ? Ok(new { status = "failed" }) : BadRequest(new { error = result.Error });
    }
}