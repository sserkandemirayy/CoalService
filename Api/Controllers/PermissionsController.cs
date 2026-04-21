using Application.DTOs.Permissions;
using Application.DTOs.Roles;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "ManageRoles")]
public class PermissionsController : BaseController
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _db; // reactivation için gerekebilir

    public PermissionsController(
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        AppDbContext db)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _db = db;
    }

    // === Create ===
    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
    {
        var permission = Permission.Create(dto.Name, dto.Description);
        await _permissionRepository.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        return Ok(PermissionDto.FromEntity(permission));
    }

    // === Read ===
    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _permissionRepository.GetAllAsync();
        return Ok(permissions.Select(PermissionDto.FromEntity));
    }

    // === Assign (reactivation-safe) ===
    [HttpPost("assign-to-role")]
    public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionDto dto, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId, ct);
        var permission = await _permissionRepository.GetByIdAsync(dto.PermissionId, ct);

        if (role == null || permission == null)
            return NotFound();

        var existing = await _db.RolePermissions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == dto.PermissionId, ct);

        if (existing != null)
        {
            // Daha önce silinmişse tekrar aktif et
            if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = CurrentUserId;
            }
        }
        else
        {
            await _db.RolePermissions.AddAsync(RolePermission.Create(dto.RoleId, dto.PermissionId), ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(new { Message = "Permission assigned successfully" });
    }

    // === Sync for Role (tam reactivation-safe) ===
    [HttpPost("sync-for-role")]
    public async Task<IActionResult> SyncPermissionsForRole([FromBody] SyncRolePermissionsDto dto, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId, ct);
        if (role == null) return NotFound();

        // 🔹 Ekleme (soft-deleted kayıt varsa reaktif et)
        foreach (var addId in dto.Add)
        {
            var existing = await _db.RolePermissions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == addId, ct);

            if (existing != null)
            {
                if (existing.DeletedAt != null)
                {
                    existing.DeletedAt = null;
                    existing.DeletedBy = null;
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.UpdatedBy = CurrentUserId;
                }
            }
            else
            {
                await _db.RolePermissions.AddAsync(RolePermission.Create(dto.RoleId, addId), ct);
            }
        }

        // 🔹 Kaldırma (soft delete)
        foreach (var removeId in dto.Remove)
        {
            var rp = await _db.RolePermissions
                .FirstOrDefaultAsync(x => x.RoleId == dto.RoleId && x.PermissionId == removeId, ct);

            if (rp != null && rp.DeletedAt == null)
            {
                rp.DeletedAt = DateTime.UtcNow;
                rp.DeletedBy = CurrentUserId;
            }
        }

        // 🔹 Concurrency hatası olmadan kaydet
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // concurrency durumunda ignore et
        }

        return Ok(new { Message = "Permissions synced successfully" });
    }
}