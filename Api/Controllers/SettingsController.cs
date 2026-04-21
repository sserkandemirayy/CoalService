using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : BaseController
{
    private readonly ISender _mediator;
    public SettingsController(ISender mediator) => _mediator = mediator;

    [HttpGet("system")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetSystemSettings(CancellationToken ct)
    {
        var settings = await _mediator.Send(new GetSettingsQuery(SettingScope.System, null), ct);
        return Ok(settings);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserSettings(CancellationToken ct)
    {
        var settings = await _mediator.Send(new GetSettingsQuery(SettingScope.User, CurrentUserId), ct);
        return Ok(settings);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertSetting([FromBody] UpsertSettingCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd with { UserId = cmd.Scope == SettingScope.User ? CurrentUserId : null }, ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
