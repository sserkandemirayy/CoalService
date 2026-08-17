using Application.Common.Models;
using Application.DTOs.EquipmentManagement;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.EquipmentManagement.Queries;

public sealed record GetEquipmentByIdQuery(
    Guid Id
) : IRequest<Result<EquipmentDto>>;

public sealed class GetEquipmentByIdQueryHandler
    : IRequestHandler<
        GetEquipmentByIdQuery,
        Result<EquipmentDto>>
{
    private readonly IEquipmentRepository _equipment;

    public GetEquipmentByIdQueryHandler(
        IEquipmentRepository equipment)
    {
        _equipment = equipment;
    }

    public async Task<Result<EquipmentDto>> Handle(
        GetEquipmentByIdQuery request,
        CancellationToken ct)
    {
        var item =
            await _equipment.GetByIdAsync(
                request.Id,
                ct);

        if (item is null)
        {
            return Result<EquipmentDto>.Failure(
                "Equipment not found.");
        }

        return Result<EquipmentDto>.Success(
            EquipmentDtoMapper.ToDto(item));
    }
}

public sealed record GetEquipmentQuery(
    string? Search,
    Guid? CompanyId,
    Guid? BranchId,
    Guid? CategoryId,
    Guid? FloorMapId,
    string? Status,
    bool? IsActive,
    int Page,
    int PageSize
) : IRequest<Result<EquipmentPagedDto>>;

public sealed class GetEquipmentQueryHandler
    : IRequestHandler<
        GetEquipmentQuery,
        Result<EquipmentPagedDto>>
{
    private readonly IEquipmentRepository _equipment;

    public GetEquipmentQueryHandler(
        IEquipmentRepository equipment)
    {
        _equipment = equipment;
    }

    public async Task<Result<EquipmentPagedDto>> Handle(
        GetEquipmentQuery request,
        CancellationToken ct)
    {
        EquipmentStatus? status = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<EquipmentStatus>(
                    request.Status,
                    true,
                    out var parsedStatus))
            {
                return Result<EquipmentPagedDto>.Failure(
                    "Invalid equipment status.");
            }

            status = parsedStatus;
        }

        var page =
            request.Page <= 0 ? 1 : request.Page;

        var pageSize =
            request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 200);

        var result =
            await _equipment.GetPagedAsync(
                request.Search,
                request.CompanyId,
                request.BranchId,
                request.CategoryId,
                request.FloorMapId,
                status,
                request.IsActive,
                page,
                pageSize,
                ct);

        var items = result.Items
            .Select(EquipmentDtoMapper.ToDto)
            .ToList();

        return Result<EquipmentPagedDto>.Success(
            new EquipmentPagedDto(
                items,
                result.Total,
                page,
                pageSize));
    }
}

public sealed record GetEquipmentMapItemsQuery(
    Guid FloorMapId
) : IRequest<Result<IReadOnlyList<EquipmentMapItemDto>>>;

public sealed class GetEquipmentMapItemsQueryHandler
    : IRequestHandler<
        GetEquipmentMapItemsQuery,
        Result<IReadOnlyList<EquipmentMapItemDto>>>
{
    private readonly IEquipmentRepository _equipment;

    public GetEquipmentMapItemsQueryHandler(
        IEquipmentRepository equipment)
    {
        _equipment = equipment;
    }

    public async Task<Result<IReadOnlyList<EquipmentMapItemDto>>> Handle(
        GetEquipmentMapItemsQuery request,
        CancellationToken ct)
    {
        var items =
            await _equipment.GetMapItemsAsync(
                request.FloorMapId,
                ct);

        var dto = items
            .Where(x =>
                x.X.HasValue &&
                x.Y.HasValue)
            .Select(x =>
                new EquipmentMapItemDto(
                    x.Id,
                    x.CategoryId,

                    x.Category.Code,
                    x.Category.Name,
                    x.Category.Icon,

                    x.Code,
                    x.Name,

                    x.Status.ToString(),

                    x.X!.Value,
                    x.Y!.Value,
                    x.Z))
            .ToList();

        return Result<IReadOnlyList<EquipmentMapItemDto>>
            .Success(dto);
    }
}