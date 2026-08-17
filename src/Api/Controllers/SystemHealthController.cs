using Application.SystemHealth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/system-health")]
[Authorize]
public sealed class SystemHealthController : BaseController
{
    private readonly ISender _mediator;

    public SystemHealthController(
        ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "ViewDashboard")]
    public async Task<IActionResult> Get(
        CancellationToken ct)
    {
        var result =
            await _mediator.Send(
                new GetSystemHealthQuery(),
                ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(
                new
                {
                    error = result.Error
                });
    }
}