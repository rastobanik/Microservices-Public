using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.MiddleWares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();

            if (exception is OperationCanceledException)
            {
                _logger.LogWarning(
                    "Request was cancelled. CorrelationId: {CorrelationId}", correlationId);

                context.Response.StatusCode = 499; // Client Closed Request
                return true;
            }

            _logger.LogError(exception,
                "Unhandled exception. CorrelationId: {CorrelationId}",
                correlationId);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error",
                Detail = "Something went wrong on the server.",
                Instance = context.Request.Path
            };

            problem.Extensions["correlationId"] = correlationId;

            context.Response.StatusCode = problem.Status.Value;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}
