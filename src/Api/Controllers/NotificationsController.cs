using Application.DTOs.Notifications;
using Application.Notifications.Commands;
using Application.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/notifications")]
[Authorize]
public class NotificationsController : BaseController
{
    private readonly ISender _mediator;

    public NotificationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetNotificationsQuery(CurrentUserId, page, pageSize),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetNotificationUnreadCountQuery(CurrentUserId),
            ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkNotificationReadCommand(CurrentUserId, id),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "read" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkAllNotificationsReadCommand(CurrentUserId),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "all_read" })
            : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new DeleteNotificationCommand(CurrentUserId, id),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "deleted" })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageSettings")]
    [HttpPost("custom")]
    public async Task<IActionResult> SendCustom(
        [FromBody] SendCustomNotificationDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SendCustomNotificationCommand(CurrentUserId, dto),
            ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageSettings")]
    [HttpPost("from-template")]
    public async Task<IActionResult> SendFromTemplate(
        [FromBody] SendFromTemplateDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SendNotificationFromTemplateCommand(CurrentUserId, dto),
            ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }
}