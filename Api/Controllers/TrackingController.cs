using Application.Tracking.Commands;
using Application.Tracking.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrackingController : BaseController
{
    private readonly ISender _mediator;

    public TrackingController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("current-locations")]
    public async Task<IActionResult> GetCurrentLocations(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? tagId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCurrentLocationsQuery(userId, tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("current-location/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentLocationByTagId(Guid tagId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCurrentLocationByTagIdQuery(tagId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("history/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetTagLocationHistory(
        Guid tagId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetTagLocationHistoryQuery(tagId, from, to, page, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTrackingDashboardQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("rebuild-current-location/{tagId:guid}")]
    public async Task<IActionResult> RebuildCurrentLocation(Guid tagId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RebuildCurrentLocationCommand(tagId), ct);

        return result.IsSuccess
            ? Ok(new { status = "rebuilt" })
            : BadRequest(new { error = result.Error });
    }
}