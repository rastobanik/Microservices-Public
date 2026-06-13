using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Products;
using ProductService.Endpoints;
using ProductService.Infrastructure;
using ProductService.MiddleWares;
using Serilog;
using Serilog.Events;
using Shared.Configuration.RateLimit;
using System.Threading.RateLimiting;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(ctx.Configuration));

    Log.Information("Starting up application");

    // connect to database
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
           options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // add memory cache
    builder.Services.AddMemoryCache();

    #region reateLimiter

    var rateLimitConfig = builder.Configuration
    .GetSection("RateLimiting")
    .Get<RateLimitingOptions>() ?? throw new Exception("Missing RateLimiting config");

    // Add rate limit
    builder.Services.AddRateLimiter(options =>
    {
        var global = rateLimitConfig.Global;
        var fixedOpt = rateLimitConfig.Fixed;

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string?>(httpContext =>
           RateLimitPartition.GetFixedWindowLimiter(
               partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString(),
               factory: partition => new FixedWindowRateLimiterOptions
               {
                   AutoReplenishment = true,
                   PermitLimit = global.PermitLimit,
                   QueueLimit =  global.QueueLimit,
                   Window = TimeSpan.FromSeconds(global.WindowSeconds)
               }));


        options.AddFixedWindowLimiter(RateLimitPolicies.Fixed, opt =>
            {
                opt.PermitLimit = fixedOpt.PermitLimit;
                opt.Window = TimeSpan.FromSeconds(fixedOpt.WindowSeconds);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = fixedOpt.QueueLimit;
            }
        );
    });

    #endregion

    // toto umoznuje pristupovat k HTTPContextu aj mimo kontrollerov pomocou DI
    builder.Services.AddHttpContextAccessor();

    // Add services to the container.
    builder.Services.AddScoped<IProductService, ProductService.Application.Products.ProductService>();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ProblemDetails (štandard RFC 7807)
    builder.Services.AddProblemDetails();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    
    var app = builder.Build();

    _ = app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (ctx, elapsed, ex) =>
        {
            var path = ctx.Request.Path;

            // 1. ignoruj noise endpoints
            if (path.StartsWithSegments("/swagger") ||
                path.StartsWithSegments("/favicon") ||
                path.StartsWithSegments("/health"))
            {
                return LogEventLevel.Debug;
            }

            // 2. errors
            if (ex != null || ctx.Response.StatusCode >= 500)
                return LogEventLevel.Error;

            // 3. slow requests - nastav threshold pre pomale requesty, v dev mode je to 1s, v produkcii 5s
            var slowRequestThreshold = app.Environment.IsDevelopment()
                                    ? 1000
                                : 5000;

            // sposobi to, ze vsetky requesty, ktore trvaju dlhsie ako 1s v dev a 5s v produkcii budu logovane ako warning, ostatne ako information
            if (elapsed > slowRequestThreshold)
                return LogEventLevel.Warning;

            return LogEventLevel.Information;
        };
        options.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
        {
            diagCtx.Set("UserAgent", httpCtx.Request.Headers.UserAgent.ToString());
            diagCtx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress?.ToString());
        };
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
       _ = app.MapOpenApi();
       _ = app.UseSwagger();
       _ = app.UseSwaggerUI();
    }

    app.UseRateLimiter();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.MapProductEndpoints();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public record Product(int Id, string Name, decimal Price);
