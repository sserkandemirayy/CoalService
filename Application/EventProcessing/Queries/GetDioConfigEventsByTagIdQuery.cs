using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetDioConfigEventsByTagIdQuery(Guid TagId, int Page = 1, int PageSize = 100)
    : IRequest<Result<IReadOnlyList<DioConfigEventDto>>>;

public sealed class GetDioConfigEventsByTagIdQueryHandler
    : IRequestHandler<GetDioConfigEventsByTagIdQuery, Result<IReadOnlyList<DioConfigEventDto>>>
{
    private readonly IDioConfigEventRepository _repo;

    public GetDioConfigEventsByTagIdQueryHandler(IDioConfigEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<DioConfigEventDto>>> Handle(GetDioConfigEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _repo.GetPagedByTagIdAsync(request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new DioConfigEventDto(
            x.Id, x.RawEventId, x.TagId, x.EventTimestamp, x.Pin, x.Direction.ToString(),
            x.PeriodicReportEnabled, x.PeriodicReportInterval, x.ReportOnChange, x.LowPassFilterJson)).ToList();

        return Result<IReadOnlyList<DioConfigEventDto>>.Success(dto);
    }
}