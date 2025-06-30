using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using PowerTaskManager.Exceptions;
using PowerTaskManager.Models;

namespace PowerTaskManager.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally and returning standardized error responses
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="logger">The logger</param>
        /// <param name="environment">The web host environment</param>
        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Invokes the middleware
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred");

            var statusCode = HttpStatusCode.InternalServerError;
            var errorCode = "internal_server_error";
            var message = "An unexpected error occurred";
            object errors = null;

            // Determine the appropriate status code and message based on the exception type
            switch (exception)
            {
                case AppException appEx:
                    // Use the error code from the application exception
                    errorCode = appEx.ErrorCode;
                    message = appEx.Message;

                    // Set the appropriate status code based on the exception type
                    switch (appEx)
                    {
                        case ResourceNotFoundException:
                            statusCode = HttpStatusCode.NotFound;
                            break;
                        case ValidationException validationEx:
                            statusCode = HttpStatusCode.BadRequest;
                            errors = validationEx.Errors;
                            break;
                        case UnauthorizedException:
                            statusCode = HttpStatusCode.Unauthorized;
                            break;
                        default:
                            statusCode = HttpStatusCode.BadRequest;
                            break;
                    }
                    break;
                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    errorCode = "invalid_argument";
                    message = argEx.Message;
                    break;
                case KeyNotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    errorCode = "resource_not_found";
                    message = notFoundEx.Message;
                    break;
                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorCode = "unauthorized";
                    message = unauthorizedEx.Message;
                    break;
                // Add more exception types as needed
            }

            // Create the error response
            var response = new ErrorResponse(
                message: message,
                code: errorCode,
                details: _environment.IsDevelopment() ? exception.ToString() : null
            );

            // Set the response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            string json;
            
            // Add validation errors if present
            if (errors != null)
            {
                var jsonResponse = new
                {
                    message,
                    code = errorCode,
                    details = _environment.IsDevelopment() ? exception.ToString() : null,
                    errors
                };

                json = JsonSerializer.Serialize(jsonResponse, options);
                await context.Response.WriteAsync(json);
                return;
            }

            json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Extension methods for the ErrorHandlingMiddleware
    /// </summary>
    public static class ErrorHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the error handling middleware to the application's request pipeline
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
