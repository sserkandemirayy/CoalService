using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record CreateTagCommand(
    string ExternalId,
    string Code,
    string? Name,
    string? SerialNumber,
    TagType TagType,
    string? MetadataJson,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken ct)
    {
        if (await _tagRepository.GetByExternalIdAsync(request.ExternalId, ct) is not null)
            return Result<Guid>.Failure("Tag external id already exists.");

        if (await _tagRepository.GetByCodeAsync(request.Code, ct) is not null)
            return Result<Guid>.Failure("Tag code already exists.");

        var tag = Tag.Create(
            request.ExternalId,
            request.Code,
            request.Name,
            request.SerialNumber,
            request.TagType,
            request.MetadataJson);

        await _tagRepository.AddAsync(tag, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(tag.Id);
    }
}