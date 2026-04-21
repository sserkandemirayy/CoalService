using Api.Controllers;
using Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AuditController : BaseController
{
    private readonly IAuditLogRepository _repo;

    public AuditController(IAuditLogRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? userId, [FromQuery] string? action, [FromQuery] DateTime? from, [FromQuery] DateTime? to, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        var logs = await _repo.GetPagedAsync(userId, action, from, to, page, pageSize, ct);
        return Ok(logs);
    }
}
