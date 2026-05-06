using Application.Common.Models;
using Application.DTOs.DeviceManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Queries;

public sealed record GetTagAssignmentsByTagIdQuery(Guid TagId) : IRequest<Result<IReadOnlyList<TagAssignmentDto>>>;

public sealed class GetTagAssignmentsByTagIdQueryHandler : IRequestHandler<GetTagAssignmentsByTagIdQuery, Result<IReadOnlyList<TagAssignmentDto>>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;

    public GetTagAssignmentsByTagIdQueryHandler(ITagAssignmentRepository tagAssignmentRepository)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
    }

    public async Task<Result<IReadOnlyList<TagAssignmentDto>>> Handle(GetTagAssignmentsByTagIdQuery request, CancellationToken ct)
    {
        var items = await _tagAssignmentRepository.GetAssignmentsByTagIdAsync(request.TagId, ct);

        var dto = items
            .Select(x => new TagAssignmentDto(
                x.Id,
                x.TagId,
                x.UserId,
                x.AssignedAt,
                x.UnassignedAt,
                x.IsPrimary,
                x.IsActiveAssignment))
            .ToList();

        return Result<IReadOnlyList<TagAssignmentDto>>.Success(dto);
    }
}