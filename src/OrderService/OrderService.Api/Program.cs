using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Application.Orders.GetOrderById;
using OrderService.Application.Orders.GetOrders;
using OrderService.Application.Orders.PlaceOrder;
using OrderService.Infrastructure.Inventory;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("OrderDb");

if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException("Connection string 'OrderDb' not configured.");
}

builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(conn));
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<PlaceOrderCommandHandler>();
builder.Services.AddScoped<GetOrderByIdQueryHandler>();
builder.Services.AddScoped<GetOrdersQueryHandler>();

// Logger category så Polly-logs er nemme at spotte
var pollyLogger = builder.Services
    .BuildServiceProvider()
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Polly.Inventory");

// 1) Retry (exponential backoff)
var retry = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        new[]
        {
            TimeSpan.FromMilliseconds(200),
            TimeSpan.FromMilliseconds(400),
            TimeSpan.FromMilliseconds(800)
        },
        onRetry: (outcome, delay, attempt, _) =>
            pollyLogger.LogWarning(
                "Retry {Attempt} after {Delay}ms. Reason={Reason}",
                attempt,
                delay.TotalMilliseconds,
                outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString())
    );

// 2) Circuit Breaker (stateful - shared)
var breaker = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 2, 
        durationOfBreak: TimeSpan.FromSeconds(10),
        onBreak: (outcome, span) =>
            pollyLogger.LogWarning(
                "CIRCUIT OPEN for {Seconds}s. Reason={Reason}",
                span.TotalSeconds,
                outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()),
        onReset: () => pollyLogger.LogWarning("CIRCUIT CLOSED"),
        onHalfOpen: () => pollyLogger.LogWarning("CIRCUIT HALF-OPEN")
    );

// HttpClient resiliency
builder.Services
    .AddHttpClient<IInventoryClient, InventoryClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Inventory:BaseUrl"]!);
        client.Timeout = TimeSpan.FromSeconds(2);
    })
    // 1) Retry først
    .AddPolicyHandler(retry)
    // 2) Breaker bagefter
    .AddPolicyHandler(breaker);


// RabbitMQ event bus
builder.Services.AddSingleton<IEventBus>(_ =>
{
    var host = builder.Configuration["RabbitMq:Host"] ?? "rabbitmq";
    return new RabbitMqEventBus(host);
});

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();