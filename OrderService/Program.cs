using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Application.Orders;
using OrderService.Application.Payments;
using OrderService.Configurations.Options;
using OrderService.Endpoints;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Http;
using OrderService.MiddleWares;
using Serilog;
using Shared.Infrastructure;


try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(ctx.Configuration));

    Log.Information("Starting up application");

    builder.Services.AddDbContext<ApplicationDBContext>(options =>
           options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // add memory cache
    builder.Services.AddMemoryCache();

    // add distributed cache
    builder.Services.AddCustomRedis(builder.Configuration);

    // rateLimiting
    builder.Services.AddCustomRateLimiting(builder.Configuration);   

    builder.Services.AddHttpContextAccessor();
    
    // nacitanie configuracie ProductClient pre DI kontajner   
    builder.Services.AddOptions<ProductClientOptions>().Bind(builder.Configuration.GetSection(ProductClientOptions.Section))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });

    builder.Services.AddTransient<CorrelationIdHandler>();

    builder.Services.AddHttpClient<OrderService.Infrastructure.Clients.ProductClient>((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<ProductClientOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseAddress);
    }).AddHttpMessageHandler<CorrelationIdHandler>();

    // Add services to the container.
    builder.Services.AddScoped<IOrderService, OrderService.Application.Orders.OrderService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ProblemDetails (štandard RFC 7807)
    builder.Services.AddProblemDetails();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    var app = builder.Build();

    app.UseCustomSerilogRequestLogging(builder.Configuration);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        _ = app.MapOpenApi();
        _ = app.UseSwagger();
        _ = app.UseSwaggerUI();
    }

    app.UseRateLimiter();

    var orders = new List<Order>();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.MapOrderEndpoints();

    app.MapPaymentEndpoint();

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

public record Order(int Id, int ProductId, int Quantity);

public record Payment(int Id, int OrderId, decimal Price);

