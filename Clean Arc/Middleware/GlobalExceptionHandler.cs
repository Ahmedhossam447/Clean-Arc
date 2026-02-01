
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Clean_Arc.Middleware
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

                // Note: OperationCanceledException is the parent of TaskCanceledException
                case OperationCanceledException:
                    context.Response.StatusCode = 499;
                    response.Status = 499;
                    response.Title = "Request Cancelled";
                    response.Detail = "The operation was cancelled by the user.";
                    _logger.LogInformation("Request cancelled by user.");
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
