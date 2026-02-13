
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Net;
using System.Security;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace CleanArc.API.Middleware
{
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                 await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);

            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ProblemDetails
            {
                Instance = context.Request.Path
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Status = (int)HttpStatusCode.BadRequest;
                    response.Title = "Validation Failed";
                    response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                    response.Extensions["errors"] = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Status = (int)HttpStatusCode.NotFound;
                    response.Title = "Resource Not Found";
                    response.Detail = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Status = (int)HttpStatusCode.Forbidden;
                    response.Title = "Forbidden";
                    response.Detail = exception.Message;
                    _logger.LogWarning("Unauthorized access attempt: {Message}", exception.Message);
                    break;

                case SecurityException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Status = (int)HttpStatusCode.Unauthorized;
                    response.Title = "Invalid Signature";
                    response.Detail = "The request signature is invalid.";
                    _logger.LogWarning("Invalid security signature: {Message}", exception.Message);
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    response.Status = (int)HttpStatusCode.ServiceUnavailable;
                    response.Title = "Service Configuration Error";
                    response.Detail = "A required service is not properly configured.";
                    _logger.LogError(exception, "Configuration error: {Message}", exception.Message);
                    break;

                // Note: OperationCanceledException is the parent of TaskCanceledException
                case OperationCanceledException:
                    context.Response.StatusCode = 499;
                    response.Status = 499;
                    response.Title = "Request Cancelled";
                    response.Detail = "The operation was cancelled by the user.";
                    _logger.LogInformation("Request cancelled by user.");
                    break;

                case DbUpdateConcurrencyException concurrencyEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Status = (int)HttpStatusCode.Conflict;
                    response.Title = "Concurrency Conflict";
                    response.Detail = "The data has been modified by another user since you loaded it. Please reload the page and try again.";
                    _logger.LogWarning("Concurrency conflict: {Message}", concurrencyEx.Message);
                    break;


                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Status = (int)HttpStatusCode.InternalServerError;
                    response.Title = "An internal server error occurred.";
                    _logger.LogError(exception, "Unhandled Exception");
                    break;
            }

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
