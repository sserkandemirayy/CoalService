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
    Guid CompanyId,
    Guid? BranchId,
    string? MetadataJson,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateTagCommandHandler
    : IRequestHandler<CreateTagCommand, Result<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateTagCommand request,
        CancellationToken ct)
    {
        // ============================================================
        // VALIDATION
        // ============================================================

        if (string.IsNullOrWhiteSpace(request.ExternalId))
            return Result<Guid>.Failure(
                "ExternalId is required.");

        if (string.IsNullOrWhiteSpace(request.Code))
            return Result<Guid>.Failure(
                "Tag code is required.");

        if (request.CompanyId == Guid.Empty)
            return Result<Guid>.Failure(
                "CompanyId is required.");

        // ============================================================
        // DUPLICATE EXTERNAL ID
        // ============================================================

        var existingByExternalId =
            await _tagRepository.GetByExternalIdAsync(
                request.ExternalId.Trim(),
                ct);

        if (existingByExternalId is not null)
        {
            return Result<Guid>.Failure(
                "Tag external id already exists.");
        }

        // ============================================================
        // DUPLICATE CODE
        // ============================================================

        var existingByCode =
            await _tagRepository.GetByCodeAsync(
                request.Code.Trim(),
                ct);

        if (existingByCode is not null)
        {
            return Result<Guid>.Failure(
                "Tag code already exists.");
        }

        // ============================================================
        // CREATE
        // ============================================================

        var tag = Tag.Create(
            request.ExternalId,
            request.Code,
            request.Name,
            request.SerialNumber,
            request.TagType,
            request.MetadataJson,
            request.CompanyId,
            request.BranchId);

        tag.CreatedBy = request.PerformedByUserId;

        await _tagRepository.AddAsync(
            tag,
            ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(tag.Id);
    }
}