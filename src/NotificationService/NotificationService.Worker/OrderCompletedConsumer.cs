using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using Shared.Contracts.Orders;

namespace NotificationService.Worker;

public sealed class OrderCompletedConsumer : BackgroundService
{
    private readonly ILogger<OrderCompletedConsumer> _logger;
    private readonly string _hostName;

    private IConnection? _connection;
    private IModel? _channel;

    public OrderCompletedConsumer(ILogger<OrderCompletedConsumer> logger, IConfiguration config)
    {
        _logger = logger;
        _hostName = config["RabbitMQ:Host"] ?? "rabbitmq";
    }

    public override Task StartAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory { HostName = _hostName, DispatchConsumersAsync = true };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "orders", type: ExchangeType.Topic, durable: true);

        _channel.QueueDeclare(
            queue: "notification-service",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(
            queue: "notification-service",
            exchange: "orders",
            routingKey: "order.completed");

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

        _logger.LogInformation("NotificationService consumer started. Host={Host}", _hostName);

        return base.StartAsync(ct);
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<OrderCompletedEvent>(json);

                if (evt is null)
                {
                    _logger.LogWarning("Received invalid message (could not deserialize). RoutingKey={Key}", ea.RoutingKey);
                    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                _logger.LogInformation("Received order.completed. OrderId={OrderId}, CustomerId={CustomerId}, Total={Total}",
                    evt.OrderId, evt.CustomerId, evt.TotalAmount);

                // Simulér “send notification”
                _logger.LogInformation("Notification sent. OrderId={OrderId}", evt.OrderId);

                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message. RoutingKey={Key}", ea.RoutingKey);
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel!.BasicConsume(
            queue: "notification-service",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken ct)
    {
        _logger.LogInformation("NotificationService consumer stopping.");
        _channel?.Close();
        _connection?.Close();
        return base.StopAsync(ct);
    }
}
