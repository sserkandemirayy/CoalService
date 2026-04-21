using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record DeactivateTagCommand(Guid Id, Guid PerformedByUserId) : IRequest<Result<Guid>>;

public sealed class DeactivateTagCommandHandler : IRequestHandler<DeactivateTagCommand, Result<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(DeactivateTagCommand request, CancellationToken ct)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id, ct);
        if (tag is null)
            return Result<Guid>.Failure("Tag not found.");

        tag.Deactivate();
        tag.UpdateAudit(request.PerformedByUserId);

        await _tagRepository.UpdateAsync(tag, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(tag.Id);
    }
}