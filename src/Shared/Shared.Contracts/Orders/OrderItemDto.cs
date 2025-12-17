namespace Shared.Contracts.Orders;

public sealed record OrderItemDto(
    string ProductId,
    int Quantity,
    decimal UnitPrice
);