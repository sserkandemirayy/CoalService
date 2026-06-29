using Application.DTOs.Notifications;
using Application.Notifications.Commands;
using Application.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/notification-templates")]
[Authorize(Policy = "ManageSettings")]
public class NotificationTemplatesController : BaseController
{
    private readonly ISender _mediator;

    public NotificationTemplatesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetNotificationTemplatesQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationTemplateDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateNotificationTemplateCommand(dto), ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateNotificationTemplateDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateNotificationTemplateCommand(id, dto), ct);

        return result.IsSuccess
            ? Ok(new { status = "updated" })
            : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new DeleteNotificationTemplateCommand(id, CurrentUserId),
            ct);

        return result.IsSuccess
            ? Ok(new { status = "deleted" })
            : BadRequest(new { error = result.Error });
    }
}