using Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/dashboard")]
[Authorize(Policy = "ViewDashboard")]
public class DashboardController : BaseController
{
    private readonly ISender _mediator;

    public DashboardController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("recent-alarms")]
    public async Task<IActionResult> GetRecentAlarms([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetRecentAlarmsQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("recent-emergencies")]
    public async Task<IActionResult> GetRecentEmergencies([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetRecentEmergenciesQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("recent-locations")]
    public async Task<IActionResult> GetRecentLocations([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetRecentLocationsQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("personnel-performance")]
    public async Task<IActionResult> GetPersonnelPerformance([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPersonnelPerformanceQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("personnel-risks")]
    public async Task<IActionResult> GetPersonnelRisks([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPersonnelRisksQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("zone-occupancy")]
    public async Task<IActionResult> GetZoneOccupancy([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetZoneOccupancyQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("system-health")]
    public async Task<IActionResult> GetSystemHealth([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetSystemHealthQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("battery-discipline")]
    public async Task<IActionResult> GetBatteryDiscipline([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetBatteryDisciplineQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("live-feed")]
    public async Task<IActionResult> GetLiveFeed([FromQuery] int take = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetLiveFeedQuery(take), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}