using NotificationService.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OrderCompletedConsumer>();

var host = builder.Build();
host.Run();
