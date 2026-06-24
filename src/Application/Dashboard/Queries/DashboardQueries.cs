using Application.Common.Models;
using Application.DTOs.Dashboard;
using MediatR;

namespace Application.Dashboard.Queries;

public sealed record GetDashboardSummaryQuery() : IRequest<Result<DashboardSummaryDto>>;

public sealed class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, Result<DashboardSummaryDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetDashboardSummaryQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DashboardSummaryDto>> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken ct)
    {
        var result = await _repository.GetSummaryAsync(ct);
        return Result<DashboardSummaryDto>.Success(result);
    }
}

public sealed record GetRecentAlarmsQuery(int Take = 10) : IRequest<Result<IReadOnlyList<RecentAlarmDto>>>;

public sealed class GetRecentAlarmsQueryHandler
    : IRequestHandler<GetRecentAlarmsQuery, Result<IReadOnlyList<RecentAlarmDto>>>
{
    private readonly IDashboardReadRepository _repository;

    public GetRecentAlarmsQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<RecentAlarmDto>>> Handle(
        GetRecentAlarmsQuery request,
        CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        var result = await _repository.GetRecentAlarmsAsync(take, ct);
        return Result<IReadOnlyList<RecentAlarmDto>>.Success(result);
    }
}

public sealed record GetRecentEmergenciesQuery(int Take = 10) : IRequest<Result<IReadOnlyList<RecentEmergencyDto>>>;

public sealed class GetRecentEmergenciesQueryHandler
    : IRequestHandler<GetRecentEmergenciesQuery, Result<IReadOnlyList<RecentEmergencyDto>>>
{
    private readonly IDashboardReadRepository _repository;

    public GetRecentEmergenciesQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<RecentEmergencyDto>>> Handle(
        GetRecentEmergenciesQuery request,
        CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        var result = await _repository.GetRecentEmergenciesAsync(take, ct);
        return Result<IReadOnlyList<RecentEmergencyDto>>.Success(result);
    }
}

public sealed record GetRecentLocationsQuery(int Take = 10) : IRequest<Result<IReadOnlyList<RecentLocationDto>>>;

public sealed class GetRecentLocationsQueryHandler
    : IRequestHandler<GetRecentLocationsQuery, Result<IReadOnlyList<RecentLocationDto>>>
{
    private readonly IDashboardReadRepository _repository;

    public GetRecentLocationsQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<RecentLocationDto>>> Handle(
        GetRecentLocationsQuery request,
        CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        var result = await _repository.GetRecentLocationsAsync(take, ct);
        return Result<IReadOnlyList<RecentLocationDto>>.Success(result);
    }
}

public sealed record GetPersonnelPerformanceQuery(int Take = 10) : IRequest<Result<PersonnelPerformanceDashboardDto>>;

public sealed class GetPersonnelPerformanceQueryHandler
    : IRequestHandler<GetPersonnelPerformanceQuery, Result<PersonnelPerformanceDashboardDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetPersonnelPerformanceQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PersonnelPerformanceDashboardDto>> Handle(GetPersonnelPerformanceQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        return Result<PersonnelPerformanceDashboardDto>.Success(
            await _repository.GetPersonnelPerformanceAsync(take, ct));
    }
}

public sealed record GetPersonnelRisksQuery(int Take = 10) : IRequest<Result<PersonnelRisksDashboardDto>>;

public sealed class GetPersonnelRisksQueryHandler
    : IRequestHandler<GetPersonnelRisksQuery, Result<PersonnelRisksDashboardDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetPersonnelRisksQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PersonnelRisksDashboardDto>> Handle(GetPersonnelRisksQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        return Result<PersonnelRisksDashboardDto>.Success(
            await _repository.GetPersonnelRisksAsync(take, ct));
    }
}

public sealed record GetZoneOccupancyQuery(int Take = 10) : IRequest<Result<ZoneOccupancyDashboardDto>>;

public sealed class GetZoneOccupancyQueryHandler
    : IRequestHandler<GetZoneOccupancyQuery, Result<ZoneOccupancyDashboardDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetZoneOccupancyQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ZoneOccupancyDashboardDto>> Handle(GetZoneOccupancyQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        return Result<ZoneOccupancyDashboardDto>.Success(
            await _repository.GetZoneOccupancyAsync(take, ct));
    }
}

public sealed record GetSystemHealthQuery(int Take = 10) : IRequest<Result<SystemHealthDashboardDto>>;

public sealed class GetSystemHealthQueryHandler
    : IRequestHandler<GetSystemHealthQuery, Result<SystemHealthDashboardDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetSystemHealthQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SystemHealthDashboardDto>> Handle(GetSystemHealthQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        return Result<SystemHealthDashboardDto>.Success(
            await _repository.GetSystemHealthAsync(take, ct));
    }
}

public sealed record GetBatteryDisciplineQuery(int Take = 10) : IRequest<Result<BatteryDisciplineDashboardDto>>;

public sealed class GetBatteryDisciplineQueryHandler
    : IRequestHandler<GetBatteryDisciplineQuery, Result<BatteryDisciplineDashboardDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetBatteryDisciplineQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BatteryDisciplineDashboardDto>> Handle(GetBatteryDisciplineQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 10 : Math.Min(request.Take, 100);
        return Result<BatteryDisciplineDashboardDto>.Success(
            await _repository.GetBatteryDisciplineAsync(take, ct));
    }
}

public sealed record GetLiveFeedQuery(int Take = 20) : IRequest<Result<LiveFeedDashboardDto>>;

public sealed class GetLiveFeedQueryHandler
    : IRequestHandler<GetLiveFeedQuery, Result<LiveFeedDashboardDto>>
{
    private readonly IDashboardReadRepository _repository;

    public GetLiveFeedQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<LiveFeedDashboardDto>> Handle(GetLiveFeedQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 20 : Math.Min(request.Take, 100);
        return Result<LiveFeedDashboardDto>.Success(
            await _repository.GetLiveFeedAsync(take, ct));
    }
}