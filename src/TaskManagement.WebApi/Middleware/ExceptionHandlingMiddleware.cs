using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation exception occurred");
            await WriteValidationProblemAsync(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            await WriteInternalServerErrorAsync(context, ex);
        }
    }

    private static async Task WriteValidationProblemAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        var problem = new ValidationProblemDetails(ex.Errors)
        {
            Title = "Validation Failed",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task WriteInternalServerErrorAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred. Please try again later.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
