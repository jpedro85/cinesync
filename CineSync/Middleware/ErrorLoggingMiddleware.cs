using CineSync.Utils.Logger;
using CineSync.Utils.Logger.Enums;

namespace CineSync.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerStrategy _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, ILoggerStrategy logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string errorMessageInside = FormatExceptionMessage(ex);
                _logger.Log(errorMessageInside, type: LogTypes.ERROR);
                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("{\"error\":\"An error occurred while processing your request.\"}");
                }
            }
        }

        private string FormatExceptionMessage(Exception ex)
        {
            var method = ex.TargetSite;
            var className = method?.DeclaringType?.FullName;

            return $"Exception thrown in {className}.{method?.Name}: {ex.Message}";
        }
    }
}
