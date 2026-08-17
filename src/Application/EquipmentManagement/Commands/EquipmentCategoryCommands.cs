using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EquipmentManagement.Commands;

public sealed record CreateEquipmentCategoryCommand(
    Guid CompanyId,
    string Code,
    string Name,
    string? Description,
    string? Icon,
    bool ShowOnMap,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateEquipmentCategoryCommandHandler
    : IRequestHandler<CreateEquipmentCategoryCommand, Result<Guid>>
{
    private readonly IEquipmentCategoryRepository _categories;
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _uow;

    public CreateEquipmentCategoryCommandHandler(
        IEquipmentCategoryRepository categories,
        ICompanyRepository companies,
        IUnitOfWork uow)
    {
        _categories = categories;
        _companies = companies;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        CreateEquipmentCategoryCommand request,
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

        var existing =
            await _categories.GetByCodeAsync(
                request.CompanyId,
                request.Code,
                ct);

        if (existing is not null)
        {
            return Result<Guid>.Failure(
                "Equipment category code already exists in this company.");
        }

        var category = EquipmentCategory.Create(
            request.CompanyId,
            request.Code,
            request.Name,
            request.Description,
            request.Icon,
            request.ShowOnMap);

        category.CreatedBy = request.PerformedByUserId;

        await _categories.AddAsync(
            category,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            category.Id);
    }
}

public sealed record UpdateEquipmentCategoryCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? Icon,
    bool ShowOnMap,
    bool IsActive,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class UpdateEquipmentCategoryCommandHandler
    : IRequestHandler<UpdateEquipmentCategoryCommand, Result<Guid>>
{
    private readonly IEquipmentCategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public UpdateEquipmentCategoryCommandHandler(
        IEquipmentCategoryRepository categories,
        IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        UpdateEquipmentCategoryCommand request,
        CancellationToken ct)
    {
        var category =
            await _categories.GetByIdAsync(
                request.Id,
                ct);

        if (category is null)
        {
            return Result<Guid>.Failure(
                "Equipment category not found.");
        }

        var existing =
            await _categories.GetByCodeAsync(
                category.CompanyId,
                request.Code,
                ct);

        if (existing is not null &&
            existing.Id != category.Id)
        {
            return Result<Guid>.Failure(
                "Equipment category code already exists in this company.");
        }

        category.Update(
            request.Code,
            request.Name,
            request.Description,
            request.Icon,
            request.ShowOnMap,
            request.IsActive);

        category.UpdateAudit(
            request.PerformedByUserId);

        await _categories.UpdateAsync(
            category,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            category.Id);
    }
}

public sealed record DeleteEquipmentCategoryCommand(
    Guid Id,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class DeleteEquipmentCategoryCommandHandler
    : IRequestHandler<DeleteEquipmentCategoryCommand, Result<Guid>>
{
    private readonly IEquipmentCategoryRepository _categories;
    private readonly IEquipmentRepository _equipment;
    private readonly IUnitOfWork _uow;

    public DeleteEquipmentCategoryCommandHandler(
        IEquipmentCategoryRepository categories,
        IEquipmentRepository equipment,
        IUnitOfWork uow)
    {
        _categories = categories;
        _equipment = equipment;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        DeleteEquipmentCategoryCommand request,
        CancellationToken ct)
    {
        var category =
            await _categories.GetByIdAsync(
                request.Id,
                ct);

        if (category is null)
        {
            return Result<Guid>.Failure(
                "Equipment category not found.");
        }

        var hasEquipment =
            _equipment.Query()
                .Any(x =>
                    x.CategoryId == category.Id &&
                    x.DeletedAt == null);

        if (hasEquipment)
        {
            return Result<Guid>.Failure(
                "Category cannot be deleted because equipment records are using it. Deactivate it instead.");
        }

        category.SoftDelete(
            request.PerformedByUserId);

        await _categories.UpdateAsync(
            category,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            category.Id);
    }
}