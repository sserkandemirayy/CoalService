using Application.Common.Models;
using Application.DTOs.DeviceManagement;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.DeviceManagment.Queries;

public sealed record GetTagsQuery(
    string? Search,
    string? Status,
    string? TagType,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyList<TagDto>>>;

public sealed class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, Result<IReadOnlyList<TagDto>>>
{
    private readonly ITagRepository _tagRepository;

    public GetTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<IReadOnlyList<TagDto>>> Handle(GetTagsQuery request, CancellationToken ct)
    {
        TagStatus? status = null;
        TagType? tagType = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<TagStatus>(request.Status, true, out var parsedStatus))
                return Result<IReadOnlyList<TagDto>>.Failure("Invalid tag status.");

            status = parsedStatus;
        }

        if (!string.IsNullOrWhiteSpace(request.TagType))
        {
            if (!Enum.TryParse<TagType>(request.TagType, true, out var parsedTagType))
                return Result<IReadOnlyList<TagDto>>.Failure("Invalid tag type.");

            tagType = parsedTagType;
        }

        var (items, _) = await _tagRepository.GetPagedAsync(
            request.Search,
            status,
            tagType,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new TagDto(
                x.Id,
                x.ExternalId,
                x.Code,
                x.Name,
                x.SerialNumber,
                x.TagType.ToString(),
                x.Status.ToString(),
                x.IsActive,
                x.BatteryLevel,
                x.LastSeenAt,
                x.LastEventAt,
                x.MetadataJson))
            .ToList();

        return Result<IReadOnlyList<TagDto>>.Success(dto);
    }
}