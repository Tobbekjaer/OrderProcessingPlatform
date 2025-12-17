using System.Text;
using System.Text.Json;
using OrderService.Application.Abstractions;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventBus(string hostName)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "orders", type: ExchangeType.Topic, durable: true);
    }

    public Task PublishAsync<T>(string topic, T message, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel.CreateBasicProperties();
        props.DeliveryMode = 2;

        _channel.BasicPublish(
            exchange: "orders",
            routingKey: topic,
            basicProperties: props,
            body: body
        );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}