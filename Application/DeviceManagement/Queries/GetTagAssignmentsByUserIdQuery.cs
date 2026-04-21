using Application.Common.Models;
using Application.DTOs.DeviceManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Queries;

public sealed record GetTagAssignmentsByUserIdQuery(Guid UserId) : IRequest<Result<IReadOnlyList<TagAssignmentDto>>>;

public sealed class GetTagAssignmentsByUserIdQueryHandler : IRequestHandler<GetTagAssignmentsByUserIdQuery, Result<IReadOnlyList<TagAssignmentDto>>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;

    public GetTagAssignmentsByUserIdQueryHandler(ITagAssignmentRepository tagAssignmentRepository)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
    }

    public async Task<Result<IReadOnlyList<TagAssignmentDto>>> Handle(GetTagAssignmentsByUserIdQuery request, CancellationToken ct)
    {
        var items = await _tagAssignmentRepository.GetAssignmentsByUserIdAsync(request.UserId, ct);

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