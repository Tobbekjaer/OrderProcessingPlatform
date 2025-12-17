namespace OrderService.Application.Orders.ReadModels;

public sealed record OrderItemReadModel(
    string ProductId,
    int Quantity,
    decimal UnitPrice
);
