using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.WebApi.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context, ICorrelationIdService correlationIdService)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        correlationIdService.CorrelationId = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await next(context);
    }
}
