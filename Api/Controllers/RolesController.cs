using Application.Companies.Queries;
using Application.DTOs.Companies;
using Application.DTOs.Permissions;
using Application.DTOs.Roles;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "ManageRoles")]
public class RolesController : BaseController
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RolesController(IRoleRepository roleRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    // === Create ===
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        var role = Role.Create(dto.Name, dto.Description);
        await _roleRepository.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();
        return Ok(RoleDto.FromEntity(role));
    }

    // === Read ===
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles.Select(RoleDto.FromEntity));
    }

    // === Read ===
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(Guid id, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync(id, ct);
        if (role == null) return NotFound();
        return Ok(RoleDto.FromEntity(role));
    }

    // === Update ===
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] CreateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        role.Update(dto.Name, dto.Description);
        await _unitOfWork.SaveChangesAsync();
        return Ok(RoleDto.FromEntity(role));
    }

    // === Soft Delete ===
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        // built-in koruması (örnek)
        if (role.Name is "admin" or "user" or "patient" or "doctor" or "staff")
            return Conflict(new { error = "Built-in role cannot be deleted" });

        role.SoftDelete(CurrentUserId);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    // === Role Permissions ===
    [HttpGet("{id:guid}/permissions")]
    public async Task<IActionResult> GetRolePermissions(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        var permissions = role.RolePermissions?
            .Select(rp => PermissionDto.FromEntity(rp.Permission))
            .ToList() ?? new();

        return Ok(permissions);
    }

    // === Role Users ===
    [HttpGet("{id:guid}/users")]
    public async Task<IActionResult> GetRoleUsers(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        var users = role.UserRoles?
            .Where(ur => ur.DeletedAt == null) // soft delete filtrele
            .Select(ur => new { ur.User.Id, ur.User.FirstName, ur.User.LastName, ur.User.Email })
            .ToList() ?? new();

        return Ok(users);
    }

    // === Assign Single Role to User ===
    [HttpPost("assigntouser")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        var role = await _roleRepository.GetByIdAsync(dto.RoleId);

        if (user == null || role == null) return NotFound();

        // Reactivate mantığı User.AssignRole içinde olmalı
        user.AssignRole(role);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { Message = "Role assigned successfully" });
    }

    // === Bulk Assign ===
    [HttpPost("assignbulk")]
    public async Task<IActionResult> AssignRolesBulk([FromBody] AssignRolesDto dto)
    {
        foreach (var userId in dto.Users)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) continue;

            foreach (var roleId in dto.Roles)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null) user.AssignRole(role); // içinde reactivation kontrolü var
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { Message = "Roles assigned successfully" });
    }

    // === Bulk Unassign ===
    [HttpPost("unassign")]
    public async Task<IActionResult> UnassignRolesBulk([FromBody] AssignRolesDto dto)
    {
        foreach (var userId in dto.Users)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) continue;

            foreach (var roleId in dto.Roles)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null) user.RemoveRole(role);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { Message = "Roles unassigned successfully" });
    }

    // === Sync Roles (add/remove aynı anda) ===
    [HttpPost("sync")]
    public async Task<IActionResult> SyncRoles([FromBody] SyncRolesDto dto)
    {
        foreach (var userId in dto.Users)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) continue;

            foreach (var addId in dto.Add)
            {
                var role = await _roleRepository.GetByIdAsync(addId);
                if (role != null) user.AssignRole(role);
            }

            foreach (var removeId in dto.Remove)
            {
                var role = await _roleRepository.GetByIdAsync(removeId);
                if (role != null) user.RemoveRole(role);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { Message = "Roles synchronized successfully" });
    }
}
