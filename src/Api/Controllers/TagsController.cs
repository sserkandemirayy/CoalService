using Application.DeviceManagment.Commands;
using Application.DeviceManagment.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController : BaseController
{
    private readonly ISender _mediator;

    public TagsController(
        ISender mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET BY ID
    // ================================================================

    [Authorize(Policy = "ViewDevices")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetTagByIdQuery(id),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new
            {
                error = result.Error
            });
    }

    // ================================================================
    // LIST
    // ================================================================

    [Authorize(Policy = "ViewDevices")]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? tagType,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result =
            await _mediator.Send(
                new GetTagsQuery(
                    search,
                    status,
                    tagType,
                    companyId,
                    branchId,
                    page,
                    pageSize),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // ================================================================
    // CREATE
    // ================================================================

    [Authorize(Policy = "ManageTags")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTagRequest request,
        CancellationToken ct)
    {
        var command =
            new CreateTagCommand(
                request.ExternalId,
                request.Code,
                request.Name,
                request.SerialNumber,
                request.TagType,
                request.CompanyId,
                request.BranchId,
                request.MetadataJson,
                CurrentUserId);

        var result =
            await _mediator.Send(
                command,
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                id = result.Value
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // ================================================================
    // UPDATE
    // ================================================================

    [Authorize(Policy = "ManageTags")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTagRequest request,
        CancellationToken ct)
    {
        var command =
            new UpdateTagCommand(
                id,
                request.Code,
                request.Name,
                request.SerialNumber,
                request.TagType,
                request.CompanyId,
                request.BranchId,
                request.MetadataJson,
                CurrentUserId);

        var result =
            await _mediator.Send(
                command,
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                status = "updated"
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // ================================================================
    // ACTIVATE
    // ================================================================

    [Authorize(Policy = "ManageTags")]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new ActivateTagCommand(
                    id,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                status = "activated"
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // ================================================================
    // DEACTIVATE
    // ================================================================

    [Authorize(Policy = "ManageTags")]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new DeactivateTagCommand(
                    id,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                status = "deactivated"
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // ================================================================
    // REQUEST DTOs
    // ================================================================

    public sealed record CreateTagRequest(
        string ExternalId,
        string Code,
        string? Name,
        string? SerialNumber,
        TagType TagType,
        Guid CompanyId,
        Guid? BranchId,
        string? MetadataJson
    );

    public sealed record UpdateTagRequest(
        string Code,
        string? Name,
        string? SerialNumber,
        TagType TagType,
        Guid CompanyId,
        Guid? BranchId,
        string? MetadataJson
    );
}