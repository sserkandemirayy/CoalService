using Application.Common.Models;
using Application.Common.SystemHealth;
using Application.DTOs.SystemHealth;
using MediatR;

namespace Application.SystemHealth.Queries;

public sealed record GetSystemHealthQuery()
    : IRequest<Result<SystemHealthDto>>;

public sealed class GetSystemHealthQueryHandler
    : IRequestHandler<GetSystemHealthQuery, Result<SystemHealthDto>>
{
    private readonly ISystemHealthService _systemHealthService;

    public GetSystemHealthQueryHandler(
        ISystemHealthService systemHealthService)
    {
        _systemHealthService = systemHealthService;
    }

    public async Task<Result<SystemHealthDto>> Handle(
        GetSystemHealthQuery request,
        CancellationToken ct)
    {
        try
        {
            var result = await _systemHealthService.GetAsync(ct);

            return Result<SystemHealthDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<SystemHealthDto>.Failure(
                $"System health could not be retrieved: {ex.Message}");
        }
    }
}