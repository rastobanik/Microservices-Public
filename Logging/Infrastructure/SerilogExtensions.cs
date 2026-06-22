using Logging.Configuration;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Logging.Infrastructure
{
    public static class SerilogExtensions
    {
        public static IApplicationBuilder UseCustomSerilogRequestLogging(this IApplicationBuilder app, IConfiguration configuration)
        {
            // nacitanie configuracie pre RateLimiting
            var cfg = configuration
                .GetSection(RequestLoggingOptions.Section)
                .Get<RequestLoggingOptions>()
                ?? throw new Exception("Missing RequestLogging config");

            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (ctx, elapsed, ex) =>
                {
                    // noise endpoints
                    if (cfg.NoisePaths?.Any(p =>
                        ctx.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)) == true)
                    {
                        return LogEventLevel.Debug;
                    }

                    // errors
                    if (ex != null || ctx.Response.StatusCode >= 500)
                        return LogEventLevel.Error;

                    if (elapsed > cfg.SlowRequestThresholdMs)
                        return LogEventLevel.Warning;

                    return LogEventLevel.Information;
                };

                options.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
                {
                    diagCtx.Set("UserAgent", httpCtx.Request.Headers.UserAgent.ToString());
                    diagCtx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress?.ToString());
                };
            });

            return app;
        }
    }
}
