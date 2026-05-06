using Microsoft.AspNetCore.Mvc;

namespace Api.Security;

public sealed class RequireIntegrationApiKeyAttribute : TypeFilterAttribute
{
    public RequireIntegrationApiKeyAttribute() : base(typeof(IntegrationApiKeyFilter))
    {
    }
}