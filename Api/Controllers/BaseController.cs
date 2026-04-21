using Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    /// <summary> If-Match header’dan RowVersion (byte[]) çözer. Yoksa null döner. </summary>
    protected byte[]? GetIfMatchRowVersion() =>
        ETagHelper.ParseIfMatch(Request.Headers["If-Match"].ToString());

    /// <summary> Response’a ETag header yazar (null veya boşsa atlar). </summary>
    protected void WriteETag(byte[]? rowVersion)
    {
        if (rowVersion == null || rowVersion.Length == 0)
            return;

        Response.Headers.ETag = ETagHelper.GenerateETag(rowVersion);
    }

    /// <summary> ProblemDetails oluşturur (RFC 7807). </summary>
    protected ObjectResult ProblemDetails(string title, string detail, int status, string code, object? errors = null)
    {
        var pd = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = status,
            Type = $"https://http.dev/problems/{code}"
        };

        if (errors != null)
            pd.Extensions["errors"] = errors;

        pd.Extensions["code"] = code;
        return StatusCode(status, pd);
    }
}