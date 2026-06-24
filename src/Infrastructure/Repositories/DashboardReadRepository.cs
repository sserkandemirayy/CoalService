using Application.Dashboard;
using Application.DTOs.Dashboard;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DashboardReadRepository : IDashboardReadRepository
{
    private readonly AppDbContext _db;

    public DashboardReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var last24Hours = now.AddHours(-24);
        var lastHour = now.AddHours(-1);
        var last5Minutes = now.AddMinutes(-5);
        var last15Minutes = now.AddMinutes(-15);

        var totalTags = await _db.Tags.CountAsync(ct);
        var activeTags = await _db.Tags.CountAsync(x => x.IsActive, ct);
        var onlineTags = await _db.Tags.CountAsync(x => x.Status == TagStatus.Online, ct);
        var offlineTags = await _db.Tags.CountAsync(x => x.Status == TagStatus.Offline, ct);
        var inactiveTags = await _db.Tags.CountAsync(x => x.Status == TagStatus.Inactive, ct);

        var assignedTags = await _db.TagAssignments
            .Where(x => x.UnassignedAt == null)
            .Select(x => x.TagId)
            .Distinct()
            .CountAsync(ct);

        var unassignedTags = totalTags - assignedTags;

        var personnelTags = await _db.Tags.CountAsync(x => x.TagType == TagType.Personnel, ct);
        var vehicleTags = await _db.Tags.CountAsync(x => x.TagType == TagType.Vehicle, ct);
        var assetTags = await _db.Tags.CountAsync(x => x.TagType == TagType.Asset, ct);

        var activeAlarms = await _db.Alarms.CountAsync(x => x.Status == AlarmStatus.Active, ct);
        var acknowledgedAlarms = await _db.Alarms.CountAsync(x => x.Status == AlarmStatus.Acknowledged, ct);
        var criticalAlarms = await _db.Alarms.CountAsync(x =>
            x.Status == AlarmStatus.Active &&
            x.Severity == AlarmSeverity.Critical, ct);

        var warningAlarms = await _db.Alarms.CountAsync(x =>
            x.Status == AlarmStatus.Active &&
            x.Severity == AlarmSeverity.Warning, ct);

        var todayAlarms = await _db.Alarms.CountAsync(x => x.StartedAt >= today, ct);
        var last24HoursAlarms = await _db.Alarms.CountAsync(x => x.StartedAt >= last24Hours, ct);

        var emergencyButtonAlarms = await _db.Alarms.CountAsync(x =>
            x.StartedAt >= last24Hours &&
            x.AlarmType == AlarmType.EmergencyButtonPressed, ct);

        var proximityAlarms = await _db.Alarms.CountAsync(x =>
            x.StartedAt >= last24Hours &&
            x.AlarmType == AlarmType.ProximityAlert, ct);

        var lowBatteryAlarms = await _db.Alarms.CountAsync(x =>
            x.StartedAt >= last24Hours &&
            x.AlarmType == AlarmType.LowBattery, ct);

        var anchorOfflineAlarms = await _db.Alarms.CountAsync(x =>
            x.StartedAt >= last24Hours &&
            x.AlarmType == AlarmType.AnchorOffline, ct);

        var currentlyLocatedTags = await _db.CurrentLocations
            .Select(x => x.TagId)
            .Distinct()
            .CountAsync(ct);

        var seenLast5Minutes = await _db.CurrentLocations
            .CountAsync(x => x.LastEventAt >= last5Minutes, ct);

        var seenLast15Minutes = await _db.CurrentLocations
            .CountAsync(x => x.LastEventAt >= last15Minutes, ct);

        var lostLocationTags = activeTags - seenLast15Minutes;
        if (lostLocationTags < 0) lostLocationTags = 0;

        var activePersonnel = await _db.CurrentLocations
            .Include(x => x.Tag)
            .CountAsync(x =>
                x.LastEventAt >= last15Minutes &&
                x.Tag.TagType == TagType.Personnel, ct);

        var activeVehicles = await _db.CurrentLocations
            .Include(x => x.Tag)
            .CountAsync(x =>
                x.LastEventAt >= last15Minutes &&
                x.Tag.TagType == TagType.Vehicle, ct);

        var activeAssets = await _db.CurrentLocations
            .Include(x => x.Tag)
            .CountAsync(x =>
                x.LastEventAt >= last15Minutes &&
                x.Tag.TagType == TagType.Asset, ct);

        var totalAnchors = await _db.Anchors.CountAsync(ct);
        var activeAnchors = await _db.Anchors.CountAsync(x => x.IsActive, ct);
        var onlineAnchors = await _db.Anchors.CountAsync(x => x.Status == AnchorStatus.Online, ct);
        var offlineAnchors = await _db.Anchors.CountAsync(x => x.Status == AnchorStatus.Offline, ct);

        var heartbeatMissingAnchors = await _db.Anchors.CountAsync(x =>
            x.IsActive &&
            (x.LastHeartbeatAt == null || x.LastHeartbeatAt < last5Minutes), ct);

        var criticalBatteryTags = await _db.Tags.CountAsync(x =>
            x.BatteryLevel != null &&
            x.BatteryLevel <= 10, ct);

        var lowBatteryTags = await _db.Tags.CountAsync(x =>
            x.BatteryLevel != null &&
            x.BatteryLevel <= 20, ct);

        var averageBatteryLevel = await _db.Tags
            .Where(x => x.BatteryLevel != null)
            .Select(x => (decimal?)x.BatteryLevel)
            .AverageAsync(ct) ?? 0;

        var batteryWarningsLast24Hours = await _db.BatteryEvents
            .CountAsync(x =>
                x.EventTimestamp >= last24Hours &&
                x.BatteryLevel <= 20, ct);

        var todayRawEvents = await _db.RawEvents.CountAsync(x => x.EventTimestamp >= today, ct);
        var lastHourRawEvents = await _db.RawEvents.CountAsync(x => x.EventTimestamp >= lastHour, ct);
        var processedEvents = await _db.RawEvents.CountAsync(x => x.ProcessingStatus == RawEventProcessingStatus.Processed, ct);
        var failedEvents = await _db.RawEvents.CountAsync(x => x.ProcessingStatus == RawEventProcessingStatus.Failed, ct);
        var pendingEvents = await _db.RawEvents.CountAsync(x => x.ProcessingStatus == RawEventProcessingStatus.Pending, ct);

        var totalProcessedOrFailed = processedEvents + failedEvents;
        var successRate = totalProcessedOrFailed == 0
            ? 100
            : Math.Round((decimal)processedEvents * 100 / totalProcessedOrFailed, 2);

        var lastEventAt = await _db.RawEvents
            .OrderByDescending(x => x.EventTimestamp)
            .Select(x => (DateTime?)x.EventTimestamp)
            .FirstOrDefaultAsync(ct);

        var anchorScore = totalAnchors == 0 ? 100 : (int)Math.Round((decimal)onlineAnchors * 100 / totalAnchors);
        var tagScore = activeTags == 0 ? 100 : (int)Math.Round((decimal)seenLast15Minutes * 100 / activeTags);
        var eventScore = (int)Math.Min(100, successRate);

        var alarmPenalty = criticalAlarms * 10 + warningAlarms * 3;
        var batteryPenalty = criticalBatteryTags * 5 + lowBatteryTags * 2;

        var score = (anchorScore + tagScore + eventScore) / 3;
        score -= alarmPenalty;
        score -= batteryPenalty;

        if (score < 0) score = 0;
        if (score > 100) score = 100;

        return new DashboardSummaryDto(
            new DashboardOperationSummaryDto(
                totalTags,
                activeTags,
                onlineTags,
                offlineTags,
                inactiveTags,
                assignedTags,
                unassignedTags,
                personnelTags,
                vehicleTags,
                assetTags
            ),
            new DashboardAlarmSummaryDto(
                activeAlarms,
                acknowledgedAlarms,
                criticalAlarms,
                warningAlarms,
                todayAlarms,
                last24HoursAlarms,
                emergencyButtonAlarms,
                proximityAlarms,
                lowBatteryAlarms,
                anchorOfflineAlarms
            ),
            new DashboardLocationSummaryDto(
                currentlyLocatedTags,
                seenLast5Minutes,
                seenLast15Minutes,
                lostLocationTags,
                activePersonnel,
                activeVehicles,
                activeAssets
            ),
            new DashboardAnchorSummaryDto(
                totalAnchors,
                activeAnchors,
                onlineAnchors,
                offlineAnchors,
                heartbeatMissingAnchors
            ),
            new DashboardBatterySummaryDto(
                criticalBatteryTags,
                lowBatteryTags,
                Math.Round(averageBatteryLevel, 2),
                batteryWarningsLast24Hours
            ),
            new DashboardEventSummaryDto(
                todayRawEvents,
                lastHourRawEvents,
                processedEvents,
                failedEvents,
                pendingEvents,
                successRate
            ),
            new DashboardSystemHealthDto(
                score,
                lastEventAt
            )
        );
    }

    public async Task<IReadOnlyList<RecentAlarmDto>> GetRecentAlarmsAsync(
        int take,
        CancellationToken ct = default)
    {
        return await _db.Alarms
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.Anchor)
            .Include(x => x.User)
            .OrderByDescending(x => x.StartedAt)
            .Take(take)
            .Select(x => new RecentAlarmDto(
                x.Id,
                x.AlarmType.ToString(),
                x.Severity.ToString(),
                x.Status.ToString(),
                x.Title,
                x.TagId,
                x.Tag != null ? x.Tag.Code : null,
                x.Tag != null ? x.Tag.Name : null,
                x.AnchorId,
                x.Anchor != null ? x.Anchor.Code : null,
                x.Anchor != null ? x.Anchor.Name : null,
                x.UserId,
                x.User != null ? (x.User.FirstName + " " + x.User.LastName).Trim() : null,
                x.StartedAt,
                x.AcknowledgedAt,
                x.EndedAt
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RecentEmergencyDto>> GetRecentEmergenciesAsync(
        int take,
        CancellationToken ct = default)
    {
        return await _db.EmergencyEvents
            .AsNoTracking()
            .Include(x => x.Tag)
            .ThenInclude(x => x.Assignments)
            .ThenInclude(x => x.User)
            .OrderByDescending(x => x.EventTimestamp)
            .Take(take)
            .Select(x => new RecentEmergencyDto(
                x.Id,
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                x.Tag.Assignments
                    .Where(a => a.UnassignedAt == null)
                    .Select(a => (Guid?)a.UserId)
                    .FirstOrDefault(),
                x.Tag.Assignments
                    .Where(a => a.UnassignedAt == null)
                    .Select(a => (a.User.FirstName + " " + a.User.LastName).Trim())
                    .FirstOrDefault(),
                x.EventTimestamp
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RecentLocationDto>> GetRecentLocationsAsync(
        int take,
        CancellationToken ct = default)
    {
        return await _db.CurrentLocations
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.User)
            .OrderByDescending(x => x.LastEventAt)
            .Take(take)
            .Select(x => new RecentLocationDto(
                    x.Id,
                    x.TagId,
                    x.Tag.Code,
                    x.Tag.Name,
                    x.Tag.TagType.ToString(),
                    x.UserId,
                    x.User != null ? (x.User.FirstName + " " + x.User.LastName).Trim() : null,
                    x.FloorMapId,
                    x.FloorMapZoneId,
                    x.X,
                    x.Y,
                    x.Z,
                    x.Accuracy,
                    x.Confidence,
                    x.LastKnownAnchorCount,
                    x.LastEventAt
                ))
            .ToListAsync(ct);
    }

    public async Task<PersonnelPerformanceDashboardDto> GetPersonnelPerformanceAsync(
    int take,
    CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var last5 = now.AddMinutes(-5);
        var last15 = now.AddMinutes(-15);
        var last30 = now.AddMinutes(-30);

        var activePersonnelNow = await _db.CurrentLocations
            .CountAsync(x => x.Tag.TagType == TagType.Personnel, ct);

        var movingLast5 = await _db.CurrentLocations
            .CountAsync(x => x.Tag.TagType == TagType.Personnel && x.LastEventAt >= last5, ct);

        var silentLast15 = await _db.CurrentLocations
            .CountAsync(x => x.Tag.TagType == TagType.Personnel && x.LastEventAt < last15, ct);

        var lostLast30 = await _db.CurrentLocations
            .CountAsync(x => x.Tag.TagType == TagType.Personnel && x.LastEventAt < last30, ct);

        var todayLocationTagIds = await _db.LocationEvents
            .Where(x => x.EventTimestamp >= today)
            .Select(x => x.TagId)
            .Distinct()
            .ToListAsync(ct);

        var personnelWithNoMovementToday = await _db.TagAssignments
            .Include(x => x.Tag)
            .CountAsync(x =>
                x.UnassignedAt == null &&
                x.Tag.TagType == TagType.Personnel &&
                !todayLocationTagIds.Contains(x.TagId), ct);

        var topTagCounts = await _db.LocationEvents
            .Where(x => x.EventTimestamp >= today)
            .GroupBy(x => x.TagId)
            .Select(g => new
            {
                TagId = g.Key,
                Count = g.Count(),
                LastSeenAt = g.Max(x => x.EventTimestamp)
            })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .ToListAsync(ct);

        var leastTagCounts = await _db.LocationEvents
            .Where(x => x.EventTimestamp >= today)
            .GroupBy(x => x.TagId)
            .Select(g => new
            {
                TagId = g.Key,
                Count = g.Count(),
                LastSeenAt = g.Max(x => x.EventTimestamp)
            })
            .OrderBy(x => x.Count)
            .Take(take)
            .ToListAsync(ct);

        var mostActive = await BuildPersonnelActivityListAsync(topTagCounts, ct);
        var leastActive = await BuildPersonnelActivityListAsync(leastTagCounts, ct);

        var longestSilent = await _db.CurrentLocations
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.User)
            .Where(x => x.Tag.TagType == TagType.Personnel)
            .OrderBy(x => x.LastEventAt)
            .Take(take)
            .Select(x => new SilentPersonnelDto(
                x.UserId,
                x.User != null ? (x.User.FirstName + " " + x.User.LastName).Trim() : null,
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                x.LastEventAt,
                (int)(now - x.LastEventAt).TotalMinutes
            ))
            .ToListAsync(ct);

        var assignedButNoLocation = await _db.TagAssignments
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.User)
            .Where(x =>
                x.UnassignedAt == null &&
                x.Tag.TagType == TagType.Personnel &&
                !_db.CurrentLocations.Any(cl => cl.TagId == x.TagId))
            .Take(take)
            .Select(x => new SilentPersonnelDto(
                x.UserId,
                (x.User.FirstName + " " + x.User.LastName).Trim(),
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                null,
                0
            ))
            .ToListAsync(ct);

        return new PersonnelPerformanceDashboardDto(
            activePersonnelNow,
            movingLast5,
            silentLast15,
            lostLast30,
            personnelWithNoMovementToday,
            mostActive,
            leastActive,
            longestSilent,
            assignedButNoLocation
        );
    }

    public async Task<PersonnelRisksDashboardDto> GetPersonnelRisksAsync(
    int take,
    CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var last24 = now.AddHours(-24);
        var last30Days = now.AddDays(-30);

        var activeEmergencyAlarms = await _db.Alarms.CountAsync(x =>
            x.Status == AlarmStatus.Active &&
            x.AlarmType == AlarmType.EmergencyButtonPressed, ct);

        var unacknowledgedAlarms = await _db.Alarms.CountAsync(x =>
            x.Status == AlarmStatus.Active, ct);

        var proximityViolationsLast24Hours = await _db.ProximityEvents.CountAsync(x =>
            x.EventTimestamp >= last24, ct);

        var personnelInDangerZones = await _db.CurrentLocations
            .CountAsync(x =>
                x.Tag.TagType == TagType.Personnel &&
                x.FloorMapZone != null &&
                (
                    x.FloorMapZone.ZoneType == FloorMapZoneType.Dangerous ||
                    x.FloorMapZone.ZoneType == FloorMapZoneType.Restricted
                ), ct);

        var riskyPersonnelGrouped = await _db.Alarms
            .AsNoTracking()
            .Where(x => x.StartedAt >= last30Days && x.UserId != null)
            .GroupBy(x => new { x.UserId, x.TagId })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.TagId,
                AlarmCount = g.Count(),
                LastAlarmAt = g.Max(x => x.StartedAt)
            })
            .OrderByDescending(x => x.AlarmCount)
            .Take(take)
            .ToListAsync(ct);

        var userIds = riskyPersonnelGrouped
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .Distinct()
            .ToList();

        var tagIds = riskyPersonnelGrouped
            .Where(x => x.TagId.HasValue)
            .Select(x => x.TagId!.Value)
            .Distinct()
            .ToList();

        var users = await _db.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName
            })
            .ToListAsync(ct);

        var tags = await _db.Tags
            .AsNoTracking()
            .Where(x => tagIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.Code,
                x.Name
            })
            .ToListAsync(ct);

        var riskyPersonnelRaw = riskyPersonnelGrouped
            .Select(x =>
            {
                var user = x.UserId.HasValue
                    ? users.FirstOrDefault(u => u.Id == x.UserId.Value)
                    : null;

                var tag = x.TagId.HasValue
                    ? tags.FirstOrDefault(t => t.Id == x.TagId.Value)
                    : null;

                return new RiskyPersonnelDto(
                    x.UserId,
                    user is null ? null : $"{user.FirstName} {user.LastName}".Trim(),
                    x.TagId,
                    tag?.Code,
                    tag?.Name,
                    x.AlarmCount,
                    x.LastAlarmAt
                );
            })
            .ToList();

        var unacknowledgedAlarmList = (await GetRecentAlarmsAsync(take * 3, ct))
            .Where(x => x.Status == AlarmStatus.Active.ToString())
            .Take(take)
            .ToList();

        var recentEmergencies = await GetRecentEmergenciesAsync(take, ct);

        var recentProximity = await _db.ProximityEvents
            .AsNoTracking()
            .OrderByDescending(x => x.EventTimestamp)
            .Take(take)
            .Select(x => new ProximityViolationDto(
                x.Id,
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                x.PeerTagId,
                x.PeerTag.Code,
                x.PeerTag.Name,
                x.Distance,
                x.Threshold,
                x.Severity.ToString(),
                x.EventTimestamp
            ))
            .ToListAsync(ct);

        return new PersonnelRisksDashboardDto(
            activeEmergencyAlarms,
            unacknowledgedAlarms,
            proximityViolationsLast24Hours,
            personnelInDangerZones,
            riskyPersonnelRaw,
            unacknowledgedAlarmList,
            recentEmergencies,
            recentProximity
        );
    }

    public async Task<ZoneOccupancyDashboardDto> GetZoneOccupancyAsync(
        int take,
        CancellationToken ct = default)
    {
        var currentLocations = await _db.CurrentLocations
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.FloorMapZone)
            .ToListAsync(ct);

        var zones = currentLocations
            .Where(x => x.FloorMapZoneId != null)
            .GroupBy(x => new
            {
                x.FloorMapZoneId,
                ZoneName = x.FloorMapZone != null ? x.FloorMapZone.Name : "Unknown",
                ZoneType = x.FloorMapZone != null ? x.FloorMapZone.ZoneType.ToString() : "Unknown"
            })
            .Select(g => new ZoneOccupancyDto(
                g.Key.FloorMapZoneId,
                g.Key.ZoneName,
                g.Key.ZoneType,
                g.Count(x => x.Tag.TagType == TagType.Personnel),
                g.Count(x => x.Tag.TagType == TagType.Vehicle),
                g.Count(x => x.Tag.TagType == TagType.Asset),
                g.Count()
            ))
            .OrderByDescending(x => x.TotalCount)
            .ToList();

        var dangerousZones = zones
            .Where(x =>
                x.ZoneType == FloorMapZoneType.Dangerous.ToString() ||
                x.ZoneType == FloorMapZoneType.Restricted.ToString())
            .OrderByDescending(x => x.TotalCount)
            .Take(take)
            .ToList();

        var recentEntries = currentLocations
            .Where(x => x.FloorMapZoneId != null)
            .OrderByDescending(x => x.LastEventAt)
            .Take(take)
            .Select(x => new ZoneRecentEntryDto(
                x.Id,
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                x.Tag.TagType.ToString(),
                x.FloorMapZoneId,
                x.FloorMapZone?.Name,
                x.LastEventAt
            ))
            .ToList();

        return new ZoneOccupancyDashboardDto(
            zones.Count,
            currentLocations.Count(x => x.FloorMapZoneId == null && x.Tag.TagType == TagType.Personnel),
            zones,
            dangerousZones,
            recentEntries
        );
    }

    public async Task<SystemHealthDashboardDto> GetSystemHealthAsync(
        int take,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var lastMinute = now.AddMinutes(-1);
        var last5 = now.AddMinutes(-5);
        var lastHour = now.AddHours(-1);

        var lastEventAt = await _db.RawEvents
            .OrderByDescending(x => x.EventTimestamp)
            .Select(x => (DateTime?)x.EventTimestamp)
            .FirstOrDefaultAsync(ct);

        var eventsLastMinute = await _db.RawEvents.CountAsync(x => x.EventTimestamp >= lastMinute, ct);
        var eventsLast5 = await _db.RawEvents.CountAsync(x => x.EventTimestamp >= last5, ct);
        var failedLastHour = await _db.RawEvents.CountAsync(x =>
            x.EventTimestamp >= lastHour &&
            x.ProcessingStatus == RawEventProcessingStatus.Failed, ct);

        var processedLastHour = await _db.RawEvents.CountAsync(x =>
            x.EventTimestamp >= lastHour &&
            x.ProcessingStatus == RawEventProcessingStatus.Processed, ct);

        var totalLastHour = processedLastHour + failedLastHour;

        var successRate = totalLastHour == 0
            ? 100
            : Math.Round((decimal)processedLastHour * 100 / totalLastHour, 2);

        var silentAnchors = await _db.Anchors.CountAsync(x =>
            x.IsActive &&
            (x.LastHeartbeatAt == null || x.LastHeartbeatAt < last5), ct);

        var offlineAnchors = await _db.Anchors.CountAsync(x => x.Status == AnchorStatus.Offline, ct);

        var latestHealth = await _db.AnchorHealthEvents
            .AsNoTracking()
            .Include(x => x.Anchor)
            .GroupBy(x => x.AnchorId)
            .Select(g => g.OrderByDescending(x => x.EventTimestamp).First())
            .ToListAsync(ct);

        var highCpu = latestHealth.Count(x => x.CpuUsage >= 80);
        var highMemory = latestHealth.Count(x => x.MemoryUsage >= 80);
        var highPacketLoss = latestHealth.Count(x => x.PacketLossRate >= 5);

        var problematicHealthAnchors = latestHealth
            .Where(x => x.CpuUsage >= 80 || x.MemoryUsage >= 80 || x.PacketLossRate >= 5)
            .Select(x => new ProblematicAnchorDto(
                x.AnchorId,
                x.Anchor.Code,
                x.Anchor.Name,
                x.Anchor.Status.ToString(),
                x.Anchor.LastHeartbeatAt,
                x.CpuUsage,
                x.MemoryUsage,
                x.PacketLossRate,
                x.EventTimestamp,
                x.PacketLossRate >= 5 ? "High packet loss" :
                x.CpuUsage >= 80 ? "High CPU" :
                x.MemoryUsage >= 80 ? "High memory" : "Health warning"
            ))
            .ToList();

        var offlineProblemAnchors = await _db.Anchors
            .AsNoTracking()
            .Where(x => x.Status == AnchorStatus.Offline ||
                        x.Status == AnchorStatus.Error ||
                        x.LastHeartbeatAt == null ||
                        x.LastHeartbeatAt < last5)
            .Select(x => new ProblematicAnchorDto(
                x.Id,
                x.Code,
                x.Name,
                x.Status.ToString(),
                x.LastHeartbeatAt,
                null,
                null,
                null,
                null,
                x.Status == AnchorStatus.Offline ? "Offline" :
                x.Status == AnchorStatus.Error ? "Error" :
                "Heartbeat missing"
            ))
            .ToListAsync(ct);

        var problematicAnchors = offlineProblemAnchors
            .Concat(problematicHealthAnchors)
            .GroupBy(x => x.AnchorId)
            .Select(g => g.First())
            .Take(take)
            .ToList();

        var totalAnchors = await _db.Anchors.CountAsync(ct);
        var onlineAnchors = await _db.Anchors.CountAsync(x => x.Status == AnchorStatus.Online, ct);

        var anchorScore = totalAnchors == 0 ? 100 : (int)Math.Round((decimal)onlineAnchors * 100 / totalAnchors);
        var eventScore = (int)Math.Min(100, successRate);
        var flowScore = eventsLast5 > 0 ? 100 : 0;

        var score = (anchorScore + eventScore + flowScore) / 3;
        score -= silentAnchors * 5;
        score -= failedLastHour * 2;
        score -= highPacketLoss * 5;

        if (score < 0) score = 0;
        if (score > 100) score = 100;

        var recentEvents = (await GetLiveFeedAsync(take, ct)).Items;

        return new SystemHealthDashboardDto(
            score,
            lastEventAt,
            eventsLastMinute,
            eventsLast5,
            failedLastHour,
            successRate,
            silentAnchors,
            offlineAnchors,
            highCpu,
            highMemory,
            highPacketLoss,
            problematicAnchors,
            recentEvents
        );
    }

    public async Task<BatteryDisciplineDashboardDto> GetBatteryDisciplineAsync(
        int take,
        CancellationToken ct = default)
    {
        var critical = await _db.Tags.CountAsync(x =>
            x.BatteryLevel != null &&
            x.BatteryLevel <= 10, ct);

        var low = await _db.Tags.CountAsync(x =>
            x.BatteryLevel != null &&
            x.BatteryLevel <= 20, ct);

        var avg = await _db.Tags
            .Where(x => x.BatteryLevel != null)
            .Select(x => (decimal?)x.BatteryLevel)
            .AverageAsync(ct) ?? 0;

        var lowBatteryPersonnelOnSite = await _db.CurrentLocations
            .Include(x => x.Tag)
            .CountAsync(x =>
                x.Tag.TagType == TagType.Personnel &&
                x.Tag.BatteryLevel != null &&
                x.Tag.BatteryLevel <= 20, ct);

        var lowest = await _db.Tags
            .AsNoTracking()
            .Where(x => x.BatteryLevel != null)
            .OrderBy(x => x.BatteryLevel)
            .Take(take)
            .Select(x => new LowBatteryTagDto(
                x.Id,
                x.Code,
                x.Name,
                x.TagType.ToString(),
                x.Assignments
                    .Where(a => a.UnassignedAt == null)
                    .Select(a => (Guid?)a.UserId)
                    .FirstOrDefault(),
                x.Assignments
                    .Where(a => a.UnassignedAt == null)
                    .Select(a => (a.User.FirstName + " " + a.User.LastName).Trim())
                    .FirstOrDefault(),
                x.BatteryLevel,
                x.LastSeenAt
            ))
            .ToListAsync(ct);

        var onSite = await _db.CurrentLocations
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.User)
            .Where(x => x.Tag.BatteryLevel != null && x.Tag.BatteryLevel <= 20)
            .OrderBy(x => x.Tag.BatteryLevel)
            .Take(take)
            .Select(x => new LowBatteryTagDto(
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                x.Tag.TagType.ToString(),
                x.UserId,
                x.User != null ? (x.User.FirstName + " " + x.User.LastName).Trim() : null,
                x.Tag.BatteryLevel,
                x.LastEventAt
            ))
            .ToListAsync(ct);

        var recentAlerts = await _db.BatteryEvents
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.Anchor)
            .Where(x => x.BatteryLevel <= 20)
            .OrderByDescending(x => x.EventTimestamp)
            .Take(take)
            .Select(x => new RecentBatteryAlertDto(
                x.Id,
                x.TagId,
                x.Tag.Code,
                x.Tag.Name,
                x.AnchorId,
                x.Anchor.Code,
                x.BatteryLevel,
                x.EventTimestamp
            ))
            .ToListAsync(ct);

        return new BatteryDisciplineDashboardDto(
            critical,
            low,
            lowBatteryPersonnelOnSite,
            Math.Round(avg, 2),
            lowest,
            onSite,
            recentAlerts
        );
    }

    public async Task<LiveFeedDashboardDto> GetLiveFeedAsync(
        int take,
        CancellationToken ct = default)
    {
        var rawEvents = await _db.RawEvents
            .AsNoTracking()
            .OrderByDescending(x => x.EventTimestamp)
            .Take(take)
            .Select(x => new LiveFeedItemDto(
                "RawEvent",
                x.EventType,
                x.EventType,
                x.ProcessingStatus.ToString(),
                x.EventTimestamp,
                x.Id,
                null,
                x.TagExternalId,
                null,
                x.AnchorExternalId
            ))
            .ToListAsync(ct);

        var alarms = await _db.Alarms
            .AsNoTracking()
            .Include(x => x.Tag)
            .Include(x => x.Anchor)
            .OrderByDescending(x => x.StartedAt)
            .Take(take)
            .Select(x => new LiveFeedItemDto(
                "Alarm",
                x.AlarmType.ToString(),
                x.Title,
                x.Severity.ToString() + " / " + x.Status.ToString(),
                x.StartedAt,
                x.Id,
                x.TagId,
                x.Tag != null ? x.Tag.Code : null,
                x.AnchorId,
                x.Anchor != null ? x.Anchor.Code : null
            ))
            .ToListAsync(ct);

        var locations = await _db.CurrentLocations
            .AsNoTracking()
            .Include(x => x.Tag)
            .OrderByDescending(x => x.LastEventAt)
            .Take(take)
            .Select(x => new LiveFeedItemDto(
                "Location",
                "LocationUpdated",
                "Location updated",
                $"X:{x.X} Y:{x.Y} Z:{x.Z}",
                x.LastEventAt,
                x.Id,
                x.TagId,
                x.Tag.Code,
                null,
                null
            ))
            .ToListAsync(ct);

        var merged = rawEvents
            .Concat(alarms)
            .Concat(locations)
            .OrderByDescending(x => x.EventAt)
            .Take(take)
            .ToList();

        return new LiveFeedDashboardDto(merged);
    }

    private async Task<IReadOnlyList<PersonnelActivityDto>> BuildPersonnelActivityListAsync(
        IEnumerable<dynamic> tagCounts,
        CancellationToken ct)
    {
        var result = new List<PersonnelActivityDto>();

        foreach (var item in tagCounts)
        {
            Guid tagId = item.TagId;
            int count = item.Count;
            DateTime? lastSeenAt = item.LastSeenAt;

            var tag = await _db.Tags
                .AsNoTracking()
                .Include(x => x.Assignments)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == tagId, ct);

            if (tag is null || tag.TagType != TagType.Personnel)
                continue;

            var assignment = tag.Assignments.FirstOrDefault(x => x.UnassignedAt == null);

            result.Add(new PersonnelActivityDto(
                assignment?.UserId,
                assignment?.User is not null
                    ? (assignment.User.FirstName + " " + assignment.User.LastName).Trim()
                    : null,
                tag.Id,
                tag.Code,
                tag.Name,
                count,
                lastSeenAt
            ));
        }

        return result;
    }
}