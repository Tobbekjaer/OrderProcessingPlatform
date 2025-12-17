using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;
using Shared.Contracts.Events;
using Shared.Contracts.Inventory;

namespace OrderService.Application.Orders.PlaceOrder;

public sealed class PlaceOrderCommandHandler
{
    private readonly IOrderRepository _orders;
    private readonly IInventoryClient _inventory;
    private readonly IEventBus _eventBus;
    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(
        IOrderRepository orders,
        IInventoryClient inventory,
        IEventBus eventBus,
        ILogger<PlaceOrderCommandHandler> logger)
    {
        _orders = orders;
        _inventory = inventory;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<PlaceOrderResult> HandleAsync(PlaceOrderCommand command, CancellationToken ct)
    {
        // 1) Map input -> domænetyper
        var customerId = new CustomerId(command.CustomerId);

        var items = command.Items.Select(i =>
            new OrderItem(
                new ProductId(i.ProductId),
                Quantity.From(i.Quantity),
                Money.From(i.UnitPrice)
            )).ToList();

        // 2) Opret ordre
        var order = Order.CreateNew(customerId, items);

        _logger.LogInformation("Order started. OrderId={OrderId}", order.Id.Value);
        _logger.LogInformation("Reserving inventory. OrderId={OrderId}", order.Id.Value);

        var lines = order.Items
            .Select(x => new ReserveInventoryLineDto(x.ProductId.Value, x.Quantity.Value))
            .ToList();

        bool reserved;
        try
        {
            reserved = await _inventory.ReserveAsync(order.Id.Value, lines, ct);
        }
        catch
        {
            _logger.LogInformation("Inventory unavailable. OrderId={OrderId}", order.Id.Value);

            order.MarkFailed();
            return new PlaceOrderResult(
                Success: false,
                StatusCode: 503,
                Error: "Inventory Service is currently unavailable."
            );
        }

        if (!reserved)
        {
            _logger.LogInformation("Not enough stock. OrderId={OrderId}", order.Id.Value);

            order.MarkFailed();
            return new PlaceOrderResult(
                Success: false,
                StatusCode: 409,
                Error: "Not enough stock"
            );
        }

        order.MarkReserved();
        _logger.LogInformation("Inventory reserved. OrderId={OrderId}", order.Id.Value);

        await _orders.AddAsync(order, ct);

        var @event = new OrderCompletedEvent(
            OrderId: order.Id.Value,
            CustomerId: order.CustomerId.Value,
            TotalAmount: order.TotalAmount.value
        );

        await _eventBus.PublishAsync("order.completed", @event, ct);
        _logger.LogInformation("Publishing event order.completed. OrderId={OrderId}", order.Id.Value);

        order.MarkCompleted();
        return new PlaceOrderResult(true, OrderId: order.Id.Value);
    }
}
