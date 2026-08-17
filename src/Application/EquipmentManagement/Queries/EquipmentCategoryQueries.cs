using Application.Common.Models;
using Application.DTOs.EquipmentManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.EquipmentManagement.Queries;

public sealed record GetEquipmentCategoriesQuery(
    Guid? CompanyId,
    bool? IsActive
) : IRequest<Result<IReadOnlyList<EquipmentCategoryDto>>>;

public sealed class GetEquipmentCategoriesQueryHandler
    : IRequestHandler<
        GetEquipmentCategoriesQuery,
        Result<IReadOnlyList<EquipmentCategoryDto>>>
{
    private readonly IEquipmentCategoryRepository _categories;

    public GetEquipmentCategoriesQueryHandler(
        IEquipmentCategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<Result<IReadOnlyList<EquipmentCategoryDto>>> Handle(
        GetEquipmentCategoriesQuery request,
        CancellationToken ct)
    {
        var items =
            await _categories.GetAllAsync(
                request.CompanyId,
                request.IsActive,
                ct);

        var dto = items
            .Select(EquipmentDtoMapper.ToDto)
            .ToList();

        return Result<IReadOnlyList<EquipmentCategoryDto>>
            .Success(dto);
    }
}

public sealed record GetEquipmentCategoryByIdQuery(
    Guid Id
) : IRequest<Result<EquipmentCategoryDto>>;

public sealed class GetEquipmentCategoryByIdQueryHandler
    : IRequestHandler<
        GetEquipmentCategoryByIdQuery,
        Result<EquipmentCategoryDto>>
{
    private readonly IEquipmentCategoryRepository _categories;

    public GetEquipmentCategoryByIdQueryHandler(
        IEquipmentCategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<Result<EquipmentCategoryDto>> Handle(
        GetEquipmentCategoryByIdQuery request,
        CancellationToken ct)
    {
        var item =
            await _categories.GetByIdAsync(
                request.Id,
                ct);

        if (item is null)
        {
            return Result<EquipmentCategoryDto>.Failure(
                "Equipment category not found.");
        }

        return Result<EquipmentCategoryDto>.Success(
            EquipmentDtoMapper.ToDto(item));
    }
}