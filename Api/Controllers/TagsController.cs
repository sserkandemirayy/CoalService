using Application.DeviceManagment.Commands;
using Application.DeviceManagment.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController : BaseController
{
    private readonly ISender _mediator;

    public TagsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTagByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? tagType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetTagsQuery(search, status, tagType, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTagRequest request,
        CancellationToken ct)
    {
        var command = new CreateTagCommand(
            request.ExternalId,
            request.Code,
            request.Name,
            request.SerialNumber,
            request.TagType,
            request.MetadataJson,
            CurrentUserId);

        var result = await _mediator.Send(command, ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTagRequest request,
        CancellationToken ct)
    {
        var command = new UpdateTagCommand(
            id,
            request.Code,
            request.Name,
            request.SerialNumber,
            request.TagType,
            request.MetadataJson,
            CurrentUserId);

        var result = await _mediator.Send(command, ct);

        return result.IsSuccess
            ? Ok(new { status = "updated" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ActivateTagCommand(id, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "activated" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeactivateTagCommand(id, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "deactivated" })
            : BadRequest(new { error = result.Error });
    }

    public sealed record CreateTagRequest(
        string ExternalId,
        string Code,
        string? Name,
        string? SerialNumber,
        TagType TagType,
        string? MetadataJson
    );

    public sealed record UpdateTagRequest(
        string Code,
        string? Name,
        string? SerialNumber,
        TagType TagType,
        string? MetadataJson
    );
}