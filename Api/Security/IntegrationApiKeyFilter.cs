using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Security;

public class IntegrationApiKeyFilter : IAsyncActionFilter
{
    private const string HeaderName = "X-Integration-Api-Key";
    private readonly IConfiguration _configuration;

    public IntegrationApiKeyFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuredKey = _configuration["IntegrationAuth:ApiKey"];

        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Integration API key is not configured." });
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Missing integration API key." });
            return;
        }

        if (!string.Equals(configuredKey, providedKey.ToString(), StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid integration API key." });
            return;
        }

        await next();
    }
}