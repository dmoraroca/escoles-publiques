using System.Text.Json;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services;
/// <summary>
/// Intercepts the HTTP pipeline to apply api exception handling behavior.
/// </summary>
public sealed class ApiExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
            /// <summary>
            /// Executes middleware logic for the current HTTP request.
            /// </summary>
            public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error for {Method} {Path} TraceId={TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            var errors = ex.Errors.Count > 0
                ? ex.Errors
                : new Dictionary<string, string[]> { ["Validation"] = new[] { ex.Message } };

            var details = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation error",
                Detail = "One or more validation errors occurred."
            };

            EnrichProblem(details, context, "validation_error");
            details.Extensions["fieldErrors"] = errors;
            await WriteProblem(context, details.Status!.Value, details);
        }
        catch (DuplicateEntityException ex)
        {
            _logger.LogWarning(ex, "Duplicate entity for {Method} {Path} TraceId={TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            var details = CreateProblem(
                StatusCodes.Status409Conflict,
                "Conflict",
                ex.Message,
                context,
                "duplicate_entity");

            await WriteProblem(context, details.Status!.Value, details);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for {Method} {Path} TraceId={TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            var details = CreateProblem(
                StatusCodes.Status404NotFound,
                "Resource not found",
                ex.Message,
                context,
                "not_found");

            await WriteProblem(context, details.Status!.Value, details);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access for {Method} {Path} TraceId={TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            var details = CreateProblem(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message,
                context,
                "unauthorized");

            await WriteProblem(context, details.Status!.Value, details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path} TraceId={TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            var details = CreateProblem(
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "S'ha produit un error inesperat.",
                context,
                "internal_error");

            await WriteProblem(context, details.Status!.Value, details);
        }
    }
            /// <summary>
            /// Creates problem by applying the required business rules.
            /// </summary>
            private static ProblemDetails CreateProblem(int status, string title, string detail, HttpContext context, string errorCode)
    {
        var details = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        EnrichProblem(details, context, errorCode);
        return details;
    }
            /// <summary>
            /// Executes the enrich problem operation as part of this component.
            /// </summary>
            private static void EnrichProblem(ProblemDetails details, HttpContext context, string errorCode)
    {
        details.Instance = context.Request.Path;
        details.Extensions["errorCode"] = errorCode;
        details.Extensions["traceId"] = context.TraceIdentifier;
        details.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    }
            /// <summary>
            /// Executes the write problem operation as part of this component.
            /// </summary>
            private static async Task WriteProblem(HttpContext context, int status, ProblemDetails details)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(context.Response.Body, details, JsonOptions);
    }
}
