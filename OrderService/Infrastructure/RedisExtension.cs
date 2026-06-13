using Microsoft.Extensions.Options;
using OrderService.Configurations.Options;
using StackExchange.Redis;

namespace OrderService.Infrastructure
{
    public static class RedisExtension
    {
        public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration configuration)
        {
            // toto nacitanie configuracie pre distributed cache Redis
            services.AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;

                var config = ConfigurationOptions.Parse(options.Configuration);
                config.AbortOnConnectFail = false;
                config.ConnectRetry = options.ConnectRetry;
                config.ReconnectRetryPolicy = new ExponentialRetry(options.ReconnectTimeOut);

                return ConnectionMultiplexer.Connect(config);
            });

            return services;
        }
    }
}
