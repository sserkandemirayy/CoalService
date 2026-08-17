using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EquipmentManagement.Commands;

public sealed record CreateEquipmentCommand(
    Guid CompanyId,
    Guid? BranchId,
    Guid CategoryId,

    string Code,
    string Name,

    string? SerialNumber,
    string? Manufacturer,
    string? Model,

    string Status,

    Guid? FloorMapId,
    decimal? X,
    decimal? Y,
    decimal? Z,

    DateTime? InstalledAt,
    DateTime? ExpirationDate,
    DateTime? NextInspectionAt,

    string? Notes,
    string? MetadataJson,

    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateEquipmentCommandHandler
    : IRequestHandler<CreateEquipmentCommand, Result<Guid>>
{
    private readonly IEquipmentRepository _equipment;
    private readonly IEquipmentCategoryRepository _categories;
    private readonly ICompanyRepository _companies;
    private readonly IBranchRepository _branches;
    private readonly IFloorMapRepository _maps;
    private readonly IUnitOfWork _uow;

    public CreateEquipmentCommandHandler(
        IEquipmentRepository equipment,
        IEquipmentCategoryRepository categories,
        ICompanyRepository companies,
        IBranchRepository branches,
        IFloorMapRepository maps,
        IUnitOfWork uow)
    {
        _equipment = equipment;
        _categories = categories;
        _companies = companies;
        _branches = branches;
        _maps = maps;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        CreateEquipmentCommand request,
        CancellationToken ct)
    {
        var company =
            await _companies.GetByIdAsync(
                request.CompanyId,
                ct);

        if (company is null)
        {
            return Result<Guid>.Failure(
                "Company not found.");
        }

        var category =
            await _categories.GetByIdAsync(
                request.CategoryId,
                ct);

        if (category is null)
        {
            return Result<Guid>.Failure(
                "Equipment category not found.");
        }

        if (category.CompanyId != request.CompanyId)
        {
            return Result<Guid>.Failure(
                "Equipment category does not belong to selected company.");
        }

        if (!category.IsActive)
        {
            return Result<Guid>.Failure(
                "Equipment category is inactive.");
        }

        if (request.BranchId.HasValue)
        {
            var branch =
                await _branches.GetByIdAsync(
                    request.BranchId.Value,
                    ct);

            if (branch is null)
            {
                return Result<Guid>.Failure(
                    "Branch not found.");
            }

            if (branch.CompanyId != request.CompanyId)
            {
                return Result<Guid>.Failure(
                    "Branch does not belong to selected company.");
            }
        }

        if (request.FloorMapId.HasValue)
        {
            var map =
                await _maps.GetByIdAsync(
                    request.FloorMapId.Value,
                    ct);

            if (map is null)
            {
                return Result<Guid>.Failure(
                    "Floor map not found.");
            }

            if (map.CompanyId.HasValue &&
                map.CompanyId.Value != request.CompanyId)
            {
                return Result<Guid>.Failure(
                    "Floor map does not belong to selected company.");
            }

            if (map.BranchId.HasValue)
            {
                if (!request.BranchId.HasValue ||
                    request.BranchId.Value != map.BranchId.Value)
                {
                    return Result<Guid>.Failure(
                        "Equipment branch must match floor map branch.");
                }
            }
        }

        var existing =
            await _equipment.GetByCodeAsync(
                request.CompanyId,
                request.Code,
                ct);

        if (existing is not null)
        {
            return Result<Guid>.Failure(
                "Equipment code already exists in this company.");
        }

        if (!Enum.TryParse<EquipmentStatus>(
                request.Status,
                true,
                out var status))
        {
            return Result<Guid>.Failure(
                "Invalid equipment status.");
        }

        Equipment entity;

        try
        {
            entity = Equipment.Create(
                request.CompanyId,
                request.BranchId,
                request.CategoryId,

                request.Code,
                request.Name,

                request.SerialNumber,
                request.Manufacturer,
                request.Model,

                status,

                request.FloorMapId,
                request.X,
                request.Y,
                request.Z,

                request.InstalledAt,
                request.ExpirationDate,
                request.NextInspectionAt,

                request.Notes,
                request.MetadataJson);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(
                ex.Message);
        }

        entity.CreatedBy =
            request.PerformedByUserId;

        await _equipment.AddAsync(
            entity,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            entity.Id);
    }
}

public sealed record UpdateEquipmentCommand(
    Guid Id,

    Guid? BranchId,
    Guid CategoryId,

    string Code,
    string Name,

    string? SerialNumber,
    string? Manufacturer,
    string? Model,

    string Status,

    Guid? FloorMapId,
    decimal? X,
    decimal? Y,
    decimal? Z,

    DateTime? InstalledAt,
    DateTime? ExpirationDate,
    DateTime? NextInspectionAt,

    string? Notes,
    string? MetadataJson,

    bool IsActive,

    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class UpdateEquipmentCommandHandler
    : IRequestHandler<UpdateEquipmentCommand, Result<Guid>>
{
    private readonly IEquipmentRepository _equipment;
    private readonly IEquipmentCategoryRepository _categories;
    private readonly IBranchRepository _branches;
    private readonly IFloorMapRepository _maps;
    private readonly IUnitOfWork _uow;

    public UpdateEquipmentCommandHandler(
        IEquipmentRepository equipment,
        IEquipmentCategoryRepository categories,
        IBranchRepository branches,
        IFloorMapRepository maps,
        IUnitOfWork uow)
    {
        _equipment = equipment;
        _categories = categories;
        _branches = branches;
        _maps = maps;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        UpdateEquipmentCommand request,
        CancellationToken ct)
    {
        var entity =
            await _equipment.GetByIdAsync(
                request.Id,
                ct);

        if (entity is null)
        {
            return Result<Guid>.Failure(
                "Equipment not found.");
        }

        var category =
            await _categories.GetByIdAsync(
                request.CategoryId,
                ct);

        if (category is null)
        {
            return Result<Guid>.Failure(
                "Equipment category not found.");
        }

        if (category.CompanyId != entity.CompanyId)
        {
            return Result<Guid>.Failure(
                "Equipment category does not belong to equipment company.");
        }

        if (request.BranchId.HasValue)
        {
            var branch =
                await _branches.GetByIdAsync(
                    request.BranchId.Value,
                    ct);

            if (branch is null)
            {
                return Result<Guid>.Failure(
                    "Branch not found.");
            }

            if (branch.CompanyId != entity.CompanyId)
            {
                return Result<Guid>.Failure(
                    "Branch does not belong to equipment company.");
            }
        }

        if (request.FloorMapId.HasValue)
        {
            var map =
                await _maps.GetByIdAsync(
                    request.FloorMapId.Value,
                    ct);

            if (map is null)
            {
                return Result<Guid>.Failure(
                    "Floor map not found.");
            }

            if (map.CompanyId.HasValue &&
                map.CompanyId.Value != entity.CompanyId)
            {
                return Result<Guid>.Failure(
                    "Floor map does not belong to equipment company.");
            }

            if (map.BranchId.HasValue)
            {
                if (!request.BranchId.HasValue ||
                    request.BranchId.Value != map.BranchId.Value)
                {
                    return Result<Guid>.Failure(
                        "Equipment branch must match floor map branch.");
                }
            }
        }

        var sameCode =
            await _equipment.GetByCodeAsync(
                entity.CompanyId,
                request.Code,
                ct);

        if (sameCode is not null &&
            sameCode.Id != entity.Id)
        {
            return Result<Guid>.Failure(
                "Equipment code already exists in this company.");
        }

        if (!Enum.TryParse<EquipmentStatus>(
                request.Status,
                true,
                out var status))
        {
            return Result<Guid>.Failure(
                "Invalid equipment status.");
        }

        try
        {
            entity.Update(
                request.BranchId,
                request.CategoryId,

                request.Code,
                request.Name,

                request.SerialNumber,
                request.Manufacturer,
                request.Model,

                status,

                request.FloorMapId,
                request.X,
                request.Y,
                request.Z,

                request.InstalledAt,
                request.ExpirationDate,
                request.NextInspectionAt,

                request.Notes,
                request.MetadataJson,

                request.IsActive);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(
                ex.Message);
        }

        entity.UpdateAudit(
            request.PerformedByUserId);

        await _equipment.UpdateAsync(
            entity,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            entity.Id);
    }
}

public sealed record DeleteEquipmentCommand(
    Guid Id,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class DeleteEquipmentCommandHandler
    : IRequestHandler<DeleteEquipmentCommand, Result<Guid>>
{
    private readonly IEquipmentRepository _equipment;
    private readonly IUnitOfWork _uow;

    public DeleteEquipmentCommandHandler(
        IEquipmentRepository equipment,
        IUnitOfWork uow)
    {
        _equipment = equipment;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        DeleteEquipmentCommand request,
        CancellationToken ct)
    {
        var entity =
            await _equipment.GetByIdAsync(
                request.Id,
                ct);

        if (entity is null)
        {
            return Result<Guid>.Failure(
                "Equipment not found.");
        }

        entity.SoftDelete(
            request.PerformedByUserId);

        await _equipment.UpdateAsync(
            entity,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            entity.Id);
    }
}