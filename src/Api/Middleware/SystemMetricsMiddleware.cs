using System.Diagnostics;
using Application.Common.SystemHealth;

namespace Api.Middleware;

public sealed class SystemMetricsMiddleware
{
    private readonly RequestDelegate _next;

    public SystemMetricsMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IApiMetricsStore metrics)
    {
        // Sadece gerçek API çağrılarını ölç.
        //
        // SignalR bağlantıları uzun süre açık kaldığından
        // API response-time ortalamasını bozmamalıdır.
        //
        // Swagger, static resource vb. de API metriğine dahil edilmez.

        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            metrics.RecordRequest(
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}