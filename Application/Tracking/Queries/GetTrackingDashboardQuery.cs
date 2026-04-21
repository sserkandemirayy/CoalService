using Application.Common.Models;
using Application.DTOs.Tracking;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetTrackingDashboardQuery() : IRequest<Result<TrackingDashboardDto>>;

public sealed class GetTrackingDashboardQueryHandler : IRequestHandler<GetTrackingDashboardQuery, Result<TrackingDashboardDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly IAlarmRepository _alarmRepository;

    public GetTrackingDashboardQueryHandler(
        ITagRepository tagRepository,
        IAnchorRepository anchorRepository,
        IAlarmRepository alarmRepository)
    {
        _tagRepository = tagRepository;
        _anchorRepository = anchorRepository;
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<TrackingDashboardDto>> Handle(GetTrackingDashboardQuery request, CancellationToken ct)
    {
        var totalTags = await _tagRepository.CountAsync(ct);
        var onlineTags = await _tagRepository.CountByStatusAsync(TagStatus.Online, ct);

        var onlineAnchors = await _anchorRepository.CountByStatusAsync(AnchorStatus.Online, ct);
        var offlineAnchors = await _anchorRepository.CountByStatusAsync(AnchorStatus.Offline, ct);
        var errorAnchors = await _anchorRepository.CountByStatusAsync(AnchorStatus.Error, ct);

        var activeAlarms = (await _alarmRepository.GetActiveAlarmsAsync(ct)).Count();

        var dto = new TrackingDashboardDto(
            totalTags,
            onlineTags,
            activeAlarms,
            onlineAnchors,
            offlineAnchors,
            errorAnchors);

        return Result<TrackingDashboardDto>.Success(dto);
    }
}