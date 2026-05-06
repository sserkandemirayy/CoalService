using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;

namespace Api.Services;

/// <summary>
/// Provides current user context info (ID, IP, roles, etc.)
/// Supports JWT fallback + background job (system user) context.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _scopeFactory;

    // Sistemsel işlemler için sabit "System" kullanıcı ID'si
    private static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    // Cache tutmak için (her request’te sadece 1 kez sorgu)
    private List<Guid>? _cachedCompanyIds;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _scopeFactory = scopeFactory;
    }

    public Guid GetCurrentUserId()
    {
        var http = _httpContextAccessor.HttpContext;

        // Normal kullanıcı (Claim)
        var claimId = http?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(claimId, out var id))
            return id;

        //  JWT fallback
        var token = http?.Request?.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var sub = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(sub, out var jwtId))
                    return jwtId;
            }
            catch { }
        }

        // Background job (system user)
        return SystemUserId;
    }

    public string? GetIpAddress()
    {
        var http = _httpContextAccessor.HttpContext;
        return http?.Connection?.RemoteIpAddress?.ToString() ?? "system";
    }

    public string? GetEmail()
    {
        var http = _httpContextAccessor.HttpContext;
        return http?.User?.FindFirstValue(ClaimTypes.Email);
    }

    public IEnumerable<string> GetRoles()
    {
        var http = _httpContextAccessor.HttpContext;
        return http?.User?.FindAll(ClaimTypes.Role).Select(c => c.Value)
               ?? Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetPermissions()
    {
        var http = _httpContextAccessor.HttpContext;
        return http?.User?.FindAll("permission").Select(c => c.Value)
               ?? Enumerable.Empty<string>();
    }

    public bool IsSystemUser() => GetCurrentUserId() == SystemUserId;

    /// <summary>
    /// NEW: Aktif kullanıcının bağlı olduğu şirket ID'lerini getirir (cache’li)
    /// Circular dependency olmadan kendi scope'unda DB erişimi yapar.
    /// </summary>
    public List<Guid> GetCurrentUserCompanyIds()
    {
        if (_cachedCompanyIds != null)
            return _cachedCompanyIds;

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty || userId == SystemUserId)
            return new List<Guid>();

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _cachedCompanyIds = db.UserCompanies
            .Where(x => x.UserId == userId)
            .Select(x => x.CompanyId)
            .ToList();

        return _cachedCompanyIds;
    }
}