using Application.EquipmentManagement.Commands;
using Application.EquipmentManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EquipmentController : BaseController
{
    private readonly ISender _mediator;

    public EquipmentController(
        ISender mediator)
    {
        _mediator = mediator;
    }

    // =========================================================
    // CATEGORIES
    // =========================================================

    [Authorize(Policy = "ViewDevices")]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
        [FromQuery] Guid? companyId,
        [FromQuery] bool? isActive,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetEquipmentCategoriesQuery(
                    companyId,
                    isActive),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ViewDevices")]
    [HttpGet("categories/{id:guid}")]
    public async Task<IActionResult> GetCategory(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetEquipmentCategoryByIdQuery(id),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateEquipmentCategoryRequest request,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new CreateEquipmentCategoryCommand(
                    request.CompanyId,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.Icon,
                    request.ShowOnMap,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                id = result.Value
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateEquipmentCategoryRequest request,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new UpdateEquipmentCategoryCommand(
                    id,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.Icon,
                    request.ShowOnMap,
                    request.IsActive,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                status = "updated"
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new DeleteEquipmentCategoryCommand(
                    id,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // =========================================================
    // EQUIPMENT
    // =========================================================

    [Authorize(Policy = "ViewDevices")]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? search,
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? floorMapId,
        [FromQuery] string? status,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result =
            await _mediator.Send(
                new GetEquipmentQuery(
                    search,
                    companyId,
                    branchId,
                    categoryId,
                    floorMapId,
                    status,
                    isActive,
                    page,
                    pageSize),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ViewDevices")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetEquipmentByIdQuery(id),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEquipmentRequest request,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new CreateEquipmentCommand(
                    request.CompanyId,
                    request.BranchId,
                    request.CategoryId,

                    request.Code,
                    request.Name,

                    request.SerialNumber,
                    request.Manufacturer,
                    request.Model,

                    request.Status,

                    request.FloorMapId,
                    request.X,
                    request.Y,
                    request.Z,

                    request.InstalledAt,
                    request.ExpirationDate,
                    request.NextInspectionAt,

                    request.Notes,
                    request.MetadataJson,

                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                id = result.Value
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEquipmentRequest request,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new UpdateEquipmentCommand(
                    id,

                    request.BranchId,
                    request.CategoryId,

                    request.Code,
                    request.Name,

                    request.SerialNumber,
                    request.Manufacturer,
                    request.Model,

                    request.Status,

                    request.FloorMapId,
                    request.X,
                    request.Y,
                    request.Z,

                    request.InstalledAt,
                    request.ExpirationDate,
                    request.NextInspectionAt,

                    request.Notes,
                    request.MetadataJson,

                    request.IsActive,

                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                status = "updated"
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new DeleteEquipmentCommand(
                    id,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // =========================================================
    // MAP
    // =========================================================

    [Authorize(Policy = "ViewDevices")]
    [HttpGet("map/{floorMapId:guid}")]
    public async Task<IActionResult> GetMapItems(
        Guid floorMapId,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetEquipmentMapItemsQuery(
                    floorMapId),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // =========================================================
    // INSPECTIONS
    // =========================================================

    [Authorize(Policy = "ViewDevices")]
    [HttpGet("{equipmentId:guid}/inspections")]
    public async Task<IActionResult> GetInspections(
        Guid equipmentId,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetEquipmentInspectionsQuery(
                    equipmentId),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new
            {
                error = result.Error
            });
    }

    [Authorize(Policy = "ManageDeviceConfigs")]
    [HttpPost("{equipmentId:guid}/inspections")]
    public async Task<IActionResult> AddInspection(
        Guid equipmentId,
        [FromBody] AddEquipmentInspectionRequest request,
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new AddEquipmentInspectionCommand(
                    equipmentId,
                    request.Result,
                    request.InspectedAt,
                    request.Note,
                    request.NextInspectionAt,
                    request.DataJson,
                    CurrentUserId),
                ct);

        return result.IsSuccess
            ? Ok(new
            {
                id = result.Value
            })
            : BadRequest(new
            {
                error = result.Error
            });
    }

    // =========================================================
    // REQUEST MODELS
    // =========================================================

    public sealed record CreateEquipmentCategoryRequest(
        Guid CompanyId,
        string Code,
        string Name,
        string? Description,
        string? Icon,
        bool ShowOnMap
    );

    public sealed record UpdateEquipmentCategoryRequest(
        string Code,
        string Name,
        string? Description,
        string? Icon,
        bool ShowOnMap,
        bool IsActive
    );

    public sealed record CreateEquipmentRequest(
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
        string? MetadataJson
    );

    public sealed record UpdateEquipmentRequest(
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

        bool IsActive
    );

    public sealed record AddEquipmentInspectionRequest(
        string Result,
        DateTime? InspectedAt,
        string? Note,
        DateTime? NextInspectionAt,
        string? DataJson
    );
}