using Application.DeviceManagment.Commands;
using Application.DeviceManagment.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnchorsController : BaseController
{
    private readonly ISender _mediator;

    public AnchorsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAnchorByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAnchorsQuery(search, status, companyId, branchId, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAnchorRequest request,
        CancellationToken ct)
    {
        var command = new CreateAnchorCommand(
            request.ExternalId,
            request.Code,
            request.Name,
            request.IpAddress,
            request.CompanyId,
            request.BranchId,
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
        [FromBody] UpdateAnchorRequest request,
        CancellationToken ct)
    {
        var command = new UpdateAnchorCommand(
            id,
            request.Code,
            request.Name,
            request.IpAddress,
            request.CompanyId,
            request.BranchId,
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
        var result = await _mediator.Send(new ActivateAnchorCommand(id, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "activated" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeactivateAnchorCommand(id, CurrentUserId), ct);

        return result.IsSuccess
            ? Ok(new { status = "deactivated" })
            : BadRequest(new { error = result.Error });
    }

    public sealed record CreateAnchorRequest(
        string ExternalId,
        string Code,
        string? Name,
        string? IpAddress,
        Guid? CompanyId,
        Guid? BranchId,
        string? MetadataJson
    );

    public sealed record UpdateAnchorRequest(
        string Code,
        string? Name,
        string? IpAddress,
        Guid? CompanyId,
        Guid? BranchId,
        string? MetadataJson
    );
}