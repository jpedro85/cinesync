using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;

namespace CineSync.Middleware
{
    /// <summary>
    /// Middleware for logging exceptions that occur during the processing of HTTP requests.
    /// </summary>
    /// <remarks>
    /// This middleware is intended to capture unhandled exceptions, log them, and provide a standard response for HTTP requests that result in errors.
    /// It logs the detailed error message using the configured logging strategy and modifies the HTTP response to indicate an internal server error.
    /// </remarks>
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerStrategy _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ErrorLoggingMiddleware"/> with the specified next delegate and logger.
        /// </summary>
        /// <param name="next">The next middleware delegate in the pipeline.</param>
        /// <param name="logger">The logger used to log exceptions.</param>
        public ErrorLoggingMiddleware(RequestDelegate next, ILoggerStrategy logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes an HTTP request, logging any unhandled exceptions that occur and modifying the response to indicate an error.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// If an exception occurs, it logs the error and if the response has not already started, sets the response status to 500 (Internal Server Error)
        /// and sends a generic error message in JSON format. This ensures that no stack traces or internal error details are sent to the client.
        /// </remarks>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string errorMessageInside = FormatExceptionMessage(ex);
                _logger.Log(errorMessageInside, LogTypes.ERROR);
                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("{\"error\":\"An error occurred while processing your request.\"}");
                }
            }
        }

        /// <summary>
        /// Formats an exception into a readable message including the class and method where it was thrown.
        /// </summary>
        /// <param name="ex">The exception to format.</param>
        /// <returns>A formatted error message.</returns>
        private string FormatExceptionMessage(Exception ex)
        {
            var method = ex.TargetSite;
            var className = method?.DeclaringType?.FullName;

            return $"Exception thrown in {className}.{method?.Name}: {ex.Message}";
        }
    }
}
