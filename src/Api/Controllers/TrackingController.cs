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

    public TrackingController(
        ISender mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // CURRENT LOCATIONS
    // ================================================================

    [Authorize(Policy = "ViewTracking")]
    [HttpGet("current-locations")]
    public async Task<IActionResult> GetCurrentLocations(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? tagId,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetCurrentLocationsQuery(
                    userId,
                    tagId),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new { error = result.Error });
    }

    [Authorize(Policy = "ViewTracking")]
    [HttpGet("current-location/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetCurrentLocationByTagId(
        Guid tagId,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetCurrentLocationByTagIdQuery(
                    tagId),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(
                new { error = result.Error });
    }

    // ================================================================
    // RAW TAG LOCATION HISTORY
    // ================================================================

    [Authorize(Policy = "ViewTrackingHistory")]
    [HttpGet("history/by-tag/{tagId:guid}")]
    public async Task<IActionResult> GetTagLocationHistory(
        Guid tagId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500,
        CancellationToken ct = default)
    {
        var result =
            await _mediator.Send(
                new GetTagLocationHistoryQuery(
                    tagId,
                    from,
                    to,
                    page,
                    pageSize),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new { error = result.Error });
    }

    // ================================================================
    // EXISTING USER MOVEMENT HISTORY
    // ================================================================

    [Authorize(Policy = "ViewTrackingHistory")]
    [HttpGet("movement-history/by-user/{userId:guid}")]
    public async Task<IActionResult> GetUserMovementHistory(
        Guid userId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? floorMapId,
        [FromQuery] Guid? floorMapZoneId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 1000,
        CancellationToken ct = default)
    {
        if (from == default ||
            to == default)
        {
            return BadRequest(new
            {
                error =
                    "'from' and 'to' are required."
            });
        }

        var result =
            await _mediator.Send(
                new GetUserMovementHistoryQuery(
                    userId,
                    from,
                    to,
                    companyId,
                    branchId,
                    floorMapId,
                    floorMapZoneId,
                    page,
                    pageSize),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new { error = result.Error });
    }

    // ================================================================
    // PERSON MOVEMENT REPORT
    // ================================================================

    [Authorize(Policy = "ViewTrackingHistory")]
    [HttpGet("movement-report/by-user/{userId:guid}")]
    public async Task<IActionResult> GetPersonMovementReport(
        Guid userId,

        [FromQuery] DateTime from,
        [FromQuery] DateTime to,

        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,

        [FromQuery] Guid? floorMapId,
        [FromQuery] Guid? floorMapZoneId,

        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 1000,

        CancellationToken ct = default)
    {
        var result =
            await _mediator.Send(
                new GetPersonMovementReportQuery(
                    userId,

                    from,
                    to,

                    companyId,
                    branchId,

                    floorMapId,
                    floorMapZoneId,

                    page,
                    pageSize),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new
                {
                    error =
                        result.Error
                });
    }

    // ================================================================
    // MOVEMENT PLAYBACK - 3D
    // ================================================================

    [Authorize(Policy = "ViewTrackingHistory")]
    [HttpGet("playback/by-user/{userId:guid}")]
    public async Task<IActionResult> GetMovementPlayback(
        Guid userId,

        [FromQuery] DateTime from,
        [FromQuery] DateTime to,

        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? floorMapId,

        [FromQuery] int maxPoints = 50000,

        CancellationToken ct = default)
    {
        var result =
            await _mediator.Send(
                new GetMovementPlaybackQuery(
                    userId,

                    from,
                    to,

                    companyId,
                    branchId,
                    floorMapId,

                    maxPoints),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new
                {
                    error =
                        result.Error
                });
    }

    // ================================================================
    // MOVEMENT 3D HEATMAP
    // ================================================================

    [Authorize(Policy = "ViewTrackingHistory")]
    [HttpGet("heatmap")]
    public async Task<IActionResult> GetMovementHeatMap(
        [FromQuery] Guid floorMapId,

        [FromQuery] DateTime from,
        [FromQuery] DateTime to,

        [FromQuery] Guid? userId,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? floorMapZoneId,

        /*
         * 3D voxel edge size.
         *
         * gridSize = 1
         * =>
         * 1m x 1m x 1m
         */
        [FromQuery] decimal gridSize = 1m,

        CancellationToken ct = default)
    {
        var result =
            await _mediator.Send(
                new GetMovementHeatMapQuery(
                    floorMapId,

                    from,
                    to,

                    userId,
                    companyId,
                    branchId,
                    floorMapZoneId,

                    gridSize),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new
                {
                    error =
                        result.Error
                });
    }

    // ================================================================
    // DASHBOARD
    // ================================================================

    [Authorize(Policy = "ViewDashboard")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetTrackingDashboardQuery(),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new { error = result.Error });
    }

    // ================================================================
    // REBUILD CURRENT LOCATION
    // ================================================================

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpPost("rebuild-current-location/{tagId:guid}")]
    public async Task<IActionResult> RebuildCurrentLocation(
        Guid tagId,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new RebuildCurrentLocationCommand(
                    tagId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                status = "rebuilt"
            })
            : BadRequest(
                new { error = result.Error });
    }
}