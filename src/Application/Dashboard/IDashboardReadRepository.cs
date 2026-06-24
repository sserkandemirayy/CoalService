using Application.DTOs.Dashboard;

namespace Application.Dashboard;

public interface IDashboardReadRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RecentAlarmDto>> GetRecentAlarmsAsync(int take, CancellationToken ct = default);
    Task<IReadOnlyList<RecentEmergencyDto>> GetRecentEmergenciesAsync(int take, CancellationToken ct = default);
    Task<IReadOnlyList<RecentLocationDto>> GetRecentLocationsAsync(int take, CancellationToken ct = default);

    Task<PersonnelPerformanceDashboardDto> GetPersonnelPerformanceAsync(int take, CancellationToken ct = default);
    Task<PersonnelRisksDashboardDto> GetPersonnelRisksAsync(int take, CancellationToken ct = default);
    Task<ZoneOccupancyDashboardDto> GetZoneOccupancyAsync(int take, CancellationToken ct = default);
    Task<SystemHealthDashboardDto> GetSystemHealthAsync(int take, CancellationToken ct = default);
    Task<BatteryDisciplineDashboardDto> GetBatteryDisciplineAsync(int take, CancellationToken ct = default);
    Task<LiveFeedDashboardDto> GetLiveFeedAsync(int take, CancellationToken ct = default);
}