using Application.Auth.Commands;
using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Persistence;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly ISender _mediator;
    private readonly AppDbContext _db;

    public AuthController(ISender mediator, AppDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserCommand cmd, CancellationToken ct)
    {
        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        await _db.SaveChangesAsync(ct);
        return Ok(res.Value);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUserCommand cmd, CancellationToken ct)
    {
        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess) return Unauthorized(new { error = res.Error });
        await _db.SaveChangesAsync(ct);
        return Ok(res.Value);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenCommand cmd, CancellationToken ct)
    {
        var res = await _mediator.Send(cmd with
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString()
        }, ct);

        if (!res.IsSuccess) return Unauthorized(new { error = res.Error });
        await _db.SaveChangesAsync(ct);
        return Ok(res.Value);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand cmd, CancellationToken ct)
    {
        var res = await _mediator.Send(cmd with
        {
            PerformedByUserId = CurrentUserId,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        }, ct);

        if (!res.IsSuccess) return BadRequest(new { error = res.Error });
        await _db.SaveChangesAsync(ct);
        return Ok();
    }
}