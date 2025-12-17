namespace OrderService.Application.Orders.PlaceOrder;

public sealed record PlaceOrderResult(
    bool Success,
    Guid? OrderId = null,
    int? StatusCode = null,
    string? Error = null
);