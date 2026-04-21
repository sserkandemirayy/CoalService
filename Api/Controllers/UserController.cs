using Application.Common.Models;
using Application.DTOs.Users;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    private readonly ISender _mediator;
    private readonly IUserTypeRepository _userTypes; // === NEW ===
    public UserController(ISender mediator, IUserTypeRepository userTypes) // === NEW ===
    {
        _mediator = mediator;
        _userTypes = userTypes;
    }

    // ===  GetMe ===
    [HttpGet("me")]
    public async Task<ActionResult> Me(CancellationToken ct)
    {
        if (CurrentUserId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new GetMeQuery(CurrentUserId), ct);
        if (!result.IsSuccess || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    // ===  GetUser (tek kişi, permission kontrolüyle) ===
    [Authorize(Policy = "ManageUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id, CurrentUserId), ct);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(new { error = result.Error });

        var dto = result.Value; // DTO zaten hazır

        // === Eğer PII görüntüleniyorsa audit at ===
        var canViewPII = dto.Permissions?.Contains("view_pii") == true
                         || User.IsInRole("admin")
                         || User.HasClaim("permission", "view_pii");

        if (canViewPII)
        {
            var audit = HttpContext.RequestServices.GetRequiredService<IAuditLogRepository>();
            await audit.AddAsync(AuditLog.Create(
                CurrentUserId,
                "view_pii",
                "User",
                id,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                new { ViewedBy = CurrentUserId, Resource = "User", TargetId = id }
            ));
            var uow = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            await uow.SaveChangesAsync(ct);
        }

        return Ok(dto);
    }


    [Authorize(Policy = "ManageUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
    [FromQuery] string? q,
    [FromQuery] string? role,
    [FromQuery] string? status,
    [FromQuery] List<string>? userTypeCodes,
    [FromQuery] Guid specializationId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = "FirstName",
    CancellationToken ct = default)
    {
        // Tek parametre "userTypeCode" varsa, listeye dönüştür
        if (Request.Query.TryGetValue("userTypeCode", out var singleCode) &&
            (userTypeCodes == null || userTypeCodes.Count == 0))
        {
            userTypeCodes = singleCode
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .SelectMany(v => (v ?? string.Empty)
                    .Split(',', '+', StringSplitOptions.RemoveEmptyEntries)) 
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();
        }

        var result = await _mediator.Send(new GetAllUsersQuery(
            CurrentUserId, q, role, status, userTypeCodes ?? new List<string>(), specializationId, page, pageSize, sort
        ), ct);

        if (!result.IsSuccess || result.Value == null)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    // === Update / Password / Role işlemleri ===
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand cmd, CancellationToken ct)
    {
        if (id != cmd.Id && !User.IsInRole("admin"))
            return Forbid();

        var result = await _mediator.Send(cmd with { Id = id, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess
            ? Ok(new { message = "User updated successfully" })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand cmd, CancellationToken ct)
    {
        if (id != cmd.UserId && !User.IsInRole("admin"))
            return Forbid();

        var result = await _mediator.Send(cmd with { UserId = id, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeactivateUserCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ActivateUserCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageRoles")]
    [HttpPost("{id}/assign-role/{roleId}")]
    public async Task<IActionResult> AssignRole(Guid id, Guid roleId, CancellationToken ct)
    {
        var result = await _mediator.Send(new AssignUserRoleCommand(id, roleId, CurrentUserId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageRoles")]
    [HttpPost("{id}/remove-role/{roleId}")]
    public async Task<IActionResult> RemoveRole(Guid id, Guid roleId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoveUserRoleCommand(id, roleId, CurrentUserId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpGet("{id}/sessions")]
    public async Task<IActionResult> GetSessions(Guid id, CancellationToken ct)
    {
        if (id != CurrentUserId && !User.IsInRole("admin"))
            return Forbid();

        var sessions = await _mediator.Send(new GetUserSessionsQuery(id), ct);
        return Ok(sessions);
    }

    [Authorize]
    [HttpDelete("{id}/sessions/{sessionId}")]
    public async Task<IActionResult> RevokeSession(Guid id, Guid sessionId, CancellationToken ct)
    {
        if (id != CurrentUserId && !User.IsInRole("admin"))
            return Forbid();

        var res = await _mediator.Send(new RevokeSessionCommand(id, sessionId), ct);
        return res.IsSuccess ? Ok() : BadRequest(new { error = res.Error });
    }

    [Authorize]
    [HttpDelete("{id}/sessions")]
    public async Task<IActionResult> RevokeAllSessions(Guid id, CancellationToken ct)
    {
        if (id != CurrentUserId && !User.IsInRole("admin"))
            return Forbid();

        var res = await _mediator.Send(new RevokeAllSessionsCommand(id), ct);
        return res.IsSuccess ? Ok() : BadRequest(new { error = res.Error });
    }

    [Authorize(Policy = "ManageRoles")]
    [HttpPost("{id}/roles:sync")]
    public async Task<IActionResult> SyncRoles(Guid id, [FromBody] SyncUserRolesCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { UserId = id, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpGet("{userId:guid}/companies")]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> GetUserCompanies(Guid userId, CancellationToken ct)
    {
        var res = await _mediator.Send(new GetUserCompaniesQuery(userId), ct);
        return res.IsSuccess ? Ok(res.Value) : NotFound(new { error = res.Error });
    }

    [HttpPost("{userId:guid}/companies/{companyId:guid}")]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> AssignUserToCompany(Guid userId, Guid companyId, CancellationToken ct)
    {
        var res = await _mediator.Send(new AssignUserToCompanyCommand(userId, companyId, CurrentUserId), ct);
        return res.IsSuccess ? Ok(new { status = "assigned" }) : BadRequest(new { error = res.Error });
    }

    [HttpDelete("{userId:guid}/companies/{companyId:guid}")]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> RemoveUserFromCompany(Guid userId, Guid companyId, CancellationToken ct)
    {
        var res = await _mediator.Send(new RemoveUserFromCompanyCommand(userId, companyId, CurrentUserId), ct);
        return res.IsSuccess ? Ok(new { status = "unassigned" }) : BadRequest(new { error = res.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{userId:guid}/companies:sync")]
    public async Task<IActionResult> SyncCompanies(Guid userId, [FromBody] SyncUserCompaniesCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { UserId = userId, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess ? Ok(new { status = "synced" }) : BadRequest(new { error = result.Error });
    }

    public record SyncUserCompaniesBody(List<Guid> CompanyIds);

    // === NEW === USER TYPES LIST ENDPOINT ===
    [Authorize(Policy = "ManageUsers")]
    [HttpGet("types")]
    public async Task<IActionResult> GetUserTypes(CancellationToken ct)
    {
        var types = await _userTypes.GetAllAsync(ct);
        var result = types.Select(t => new
        {
            t.Id,
            t.Code,
            t.Name,
            t.Description
        });
        return Ok(result);
    }
    // === NEW === CHANGE USER TYPE ===
    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{id:guid}/change-type/{userTypeId:guid}")]
    public async Task<IActionResult> ChangeUserType(Guid id, Guid userTypeId, CancellationToken ct)
    {
        var cmd = new ChangeUserTypeCommand(id, userTypeId, CurrentUserId);
        var result = await _mediator.Send(cmd, ct);

        return result.IsSuccess
            ? Ok(new { message = "User type updated successfully" })
            : BadRequest(new { error = result.Error });
    }

    // ================================================================
    // USER - BRANCH OPERATIONS
    // ================================================================

    [Authorize(Policy = "ManageUsers")]
    [HttpGet("{userId:guid}/branches")]
    public async Task<IActionResult> GetUserBranches(
      Guid userId,
      [FromQuery] Guid? companyId,
      CancellationToken ct)
    {
        var query = companyId.HasValue
            ? new GetUserBranchesByCompanyQuery(userId, companyId)
            : new GetUserBranchesByCompanyQuery(userId, null);

        var result = await _mediator.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{userId:guid}/branches/{branchId:guid}")]
    public async Task<IActionResult> AssignUserBranch(Guid userId, Guid branchId, CancellationToken ct)
    {
        var res = await _mediator.Send(
            new AssignRemoveUserBranchCommand(userId, branchId) with
            { /* PerformedBy user handler içinde alınıyor */ },
            ct
        );

        return res.IsSuccess
            ? Ok(new { status = "branch_assigned" })
            : BadRequest(new { error = res.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpDelete("{userId:guid}/branches/{branchId:guid}")]
    public async Task<IActionResult> RemoveUserBranch(Guid userId, Guid branchId, CancellationToken ct)
    {
        var res = await _mediator.Send(
            new RemoveBranchCommand(userId, branchId),
            ct
        );

        return res.IsSuccess
            ? Ok(new { status = "branch_removed" })
            : BadRequest(new { error = res.Error });
    }

    public record SyncUserBranchesBody(List<Guid> BranchIds);

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{userId:guid}/companies/{companyId:guid}/branches:sync")]
    public async Task<IActionResult> SyncUserBranches(
    Guid userId,
    Guid companyId,
    [FromBody] SyncUserBranchesBody body,
    CancellationToken ct)
    {
        var res = await _mediator.Send(
            new SyncUserBranchesCommand(
                userId,
                companyId,
                body.BranchIds,
                CurrentUserId
            ),
            ct
        );

        return res.IsSuccess
            ? Ok(new { status = "branches_synced" })
            : BadRequest(new { error = res.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpGet("specializations")]
    public async Task<IActionResult> GetSpecializations(
     [FromQuery] Guid? userTypeId,
     CancellationToken ct)
    {
        Result<IEnumerable<UserSpecializationDto>> result;

        if (userTypeId.HasValue)
        {
            result = await _mediator.Send(
                new GetUserSpecializationsByUserTypeQuery(userTypeId.Value), ct
            );
        }
        else
        {
            result = await _mediator.Send(
                new GetAllUserSpecializationsQuery(), ct
            );
        }

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("specializations")]
    public async Task<IActionResult> CreateSpecialization(
    [FromBody] CreateUserSpecializationCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPut("specializations/{id:guid}")]
    public async Task<IActionResult> UpdateSpecialization(
    Guid id,
    [FromBody] UpdateUserSpecializationCommand cmd,
    CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { Id = id }, ct);

        return result.IsSuccess
            ? Ok(new { status = "updated" })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpDelete("specializations/{id:guid}")]
    public async Task<IActionResult> DeleteSpecialization(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new DeleteUserSpecializationCommand(id, CurrentUserId),
            ct
        );

        return result.IsSuccess
            ? Ok(new { status = "deleted" })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with
        {
            PerformedByUserId = CurrentUserId
        }, ct);

        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }


}