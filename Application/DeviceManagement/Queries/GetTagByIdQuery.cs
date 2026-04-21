using Application.Common.Models;
using Application.DTOs.DeviceManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Queries;

public sealed record GetTagByIdQuery(Guid Id) : IRequest<Result<TagDto>>;

public sealed class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, Result<TagDto>>
{
    private readonly ITagRepository _tagRepository;

    public GetTagByIdQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<TagDto>> Handle(GetTagByIdQuery request, CancellationToken ct)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id, ct);
        if (tag is null)
            return Result<TagDto>.Failure("Tag not found.");

        var dto = new TagDto(
            tag.Id,
            tag.ExternalId,
            tag.Code,
            tag.Name,
            tag.SerialNumber,
            tag.TagType.ToString(),
            tag.Status.ToString(),
            tag.IsActive,
            tag.BatteryLevel,
            tag.LastSeenAt,
            tag.LastEventAt,
            tag.MetadataJson);

        return Result<TagDto>.Success(dto);
    }
}