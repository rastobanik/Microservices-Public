using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration.RateLimit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

namespace Shared.Infrastructure
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // nacitanie configuracie pre RateLimiting
            var cfg = configuration
                .GetSection(RateLimitingOptions.Section)
                .Get<RateLimitingOptions>()
                ?? throw new Exception("Missing RateLimiting config");

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string?>(httpContext =>
                {
                    var key = httpContext.User.Identity?.Name
                              ?? httpContext.Connection.RemoteIpAddress?.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        key,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = cfg.Global.PermitLimit,
                            QueueLimit = cfg.Global.QueueLimit,
                            Window = TimeSpan.FromSeconds(cfg.Global.WindowSeconds)
                        });
                });

                options.AddFixedWindowLimiter(RateLimitPolicies.Fixed, opt =>
                {
                    opt.PermitLimit = cfg.Fixed.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(cfg.Fixed.WindowSeconds);
                    opt.QueueLimit = cfg.Fixed.QueueLimit;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            return services;
        }
    }
}
