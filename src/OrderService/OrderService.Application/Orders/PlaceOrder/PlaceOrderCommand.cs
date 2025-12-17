namespace OrderService.Application.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<PlaceOrderItem> Items
);

public sealed record PlaceOrderItem(
    string ProductId,
    int Quantity,
    decimal UnitPrice
);