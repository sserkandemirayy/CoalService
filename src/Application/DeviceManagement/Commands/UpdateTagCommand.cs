using Application.Common.Models;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record UpdateTagCommand(
    Guid Id,
    string Code,
    string? Name,
    string? SerialNumber,
    TagType TagType,
    Guid CompanyId,
    Guid? BranchId,
    string? MetadataJson,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class UpdateTagCommandHandler
    : IRequestHandler<UpdateTagCommand, Result<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        UpdateTagCommand request,
        CancellationToken ct)
    {
        if (request.Id == Guid.Empty)
            return Result<Guid>.Failure(
                "Tag id is required.");

        if (string.IsNullOrWhiteSpace(request.Code))
            return Result<Guid>.Failure(
                "Tag code is required.");

        if (request.CompanyId == Guid.Empty)
            return Result<Guid>.Failure(
                "CompanyId is required.");

        var tag = await _tagRepository
            .GetByIdAsync(
                request.Id,
                ct);

        if (tag is null)
            return Result<Guid>.Failure(
                "Tag not found.");

        var duplicate =
            await _tagRepository.GetByCodeAsync(
                request.Code.Trim(),
                ct);

        if (duplicate is not null &&
            duplicate.Id != request.Id)
        {
            return Result<Guid>.Failure(
                "Tag code already exists.");
        }

        tag.UpdateInfo(
            request.Code,
            request.Name,
            request.SerialNumber,
            request.TagType,
            request.MetadataJson,
            request.CompanyId,
            request.BranchId);

        tag.UpdateAudit(
            request.PerformedByUserId);

        await _tagRepository.UpdateAsync(
            tag,
            ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(tag.Id);
    }
}