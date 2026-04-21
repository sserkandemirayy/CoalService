using Application.Branches.Commands;
using Application.Branches.Queries;
using Application.Companies.Commands;
using Application.Companies.Queries;
using Application.DTOs.Companies;
using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyController : BaseController
{
    private readonly ISender _mediator;
    public CompanyController(ISender mediator) => _mediator = mediator;

    // ==== COMPANIES ====

    [Authorize(Policy = "ManageCompanies")]
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageCompanies")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { Id = id, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageCompanies")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id, CurrentUserId), ct);
        return result.IsSuccess
            ? Ok(CompanyDto.FromEntity(result.Value!))
            : NotFound(new { error = result.Error });
    }

    [Authorize(Policy = "ManageCompanies")]
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllCompaniesQuery(CurrentUserId), ct);
        return Ok(result.Value!.Select(CompanyDto.FromEntity));
    }

    [Authorize(Policy = "ManageCompanies")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCompanyCommand(id, CurrentUserId), ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { error = result.Error });
    }

    // ==== BRANCHES ====

    [Authorize(Policy = "ManageBranches")]
    [HttpPost("{companyId}/branches")]
    public async Task<IActionResult> CreateBranch(Guid companyId, [FromBody] CreateBranchCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { CompanyId = companyId, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess
            ? Ok(new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageBranches")]
    [HttpPut("branches/{id}")]
    public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] UpdateBranchCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { Id = id, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { error = result.Error });
    }

    [Authorize(Policy = "ManageBranches")]
    [HttpGet("branches/{id}")]
    public async Task<IActionResult> GetBranch(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBranchByIdQuery(id, CurrentUserId), ct);
        return result.IsSuccess
            ? Ok(BranchDto.FromEntity(result.Value!))
            : NotFound(new { error = result.Error });
    }

    [Authorize(Policy = "ManageBranches")]
    [HttpGet("{companyId}/branches")]
    public async Task<IActionResult> GetBranches(Guid companyId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBranchesByCompanyIdQuery(companyId, CurrentUserId), ct);
        return Ok(result.Value!.Select(BranchDto.FromEntity));
    }

    [Authorize(Policy = "ManageBranches")]
    [HttpDelete("branches/{id}")]
    public async Task<IActionResult> DeleteBranch(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteBranchCommand(id, CurrentUserId), ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { error = result.Error });
    }

    // === GET: Şirkete bağlı kullanıcılar ===
    [HttpGet("{companyId:guid}/users")]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> GetCompanyUsers(Guid companyId, CancellationToken ct)
    {
        var res = await _mediator.Send(new GetCompanyUsersQuery(companyId), ct);
        return res.IsSuccess ? Ok(res.Value) : NotFound(new { error = res.Error });
    }

    // === POST: Şirkete kullanıcı ata ===
    [HttpPost("{companyId:guid}/users/{userId:guid}")]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> AssignUser(Guid companyId, Guid userId, CancellationToken ct)
    {
        var cmd = new AssignUserToCompanyCommand(userId, companyId, CurrentUserId);
        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess)
            return BadRequest(new { error = res.Error });

        return Ok(new { status = "assigned" });
    }

    // === DELETE: Şirketten kullanıcı çıkar ===
    [HttpDelete("{companyId:guid}/users/{userId:guid}")]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> RemoveUser(Guid companyId, Guid userId, CancellationToken ct)
    {
        var cmd = new RemoveUserFromCompanyCommand(userId, companyId, CurrentUserId);
        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess)
            return BadRequest(new { error = res.Error });

        return Ok(new { status = "unassigned" });
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("{companyId:guid}/users:sync")]
    public async Task<IActionResult> SyncUsers(Guid companyId, [FromBody] SyncCompanyUsersCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { CompanyId = companyId, PerformedByUserId = CurrentUserId }, ct);
        return result.IsSuccess ? Ok(new { status = "synced" }) : BadRequest(new { error = result.Error });
    }

    public record SyncCompanyUsersBody(List<Guid> UserIds);
}
