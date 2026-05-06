using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record AssignTagToUserCommand(
    Guid TagId,
    Guid UserId,
    bool IsPrimary,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class AssignTagToUserCommandHandler : IRequestHandler<AssignTagToUserCommand, Result<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTagToUserCommandHandler(
        ITagRepository tagRepository,
        IUserRepository userRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AssignTagToUserCommand request, CancellationToken ct)
    {
        var tag = await _tagRepository.GetByIdAsync(request.TagId, ct);
        if (tag is null)
            return Result<Guid>.Failure("Tag not found.");

        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<Guid>.Failure("User not found.");

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(request.TagId, ct);
        if (activeAssignment is not null)
        {
            activeAssignment.Unassign();
            activeAssignment.UpdateAudit(request.PerformedByUserId);
            await _tagAssignmentRepository.UpdateAsync(activeAssignment, ct);
        }

        var newAssignment = TagAssignment.Create(request.TagId, request.UserId, request.IsPrimary);
        await _tagAssignmentRepository.AddAsync(newAssignment, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(newAssignment.Id);
    }
}