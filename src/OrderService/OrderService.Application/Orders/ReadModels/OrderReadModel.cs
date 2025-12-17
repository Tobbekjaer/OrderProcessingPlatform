namespace OrderService.Application.Orders.ReadModels;

public sealed record OrderReadModel(
    Guid OrderId,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<OrderItemReadModel> Items
);