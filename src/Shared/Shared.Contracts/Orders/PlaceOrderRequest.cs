namespace Shared.Contracts.Orders;

public sealed record PlaceOrderRequest(
    Guid CustomerId,
    IReadOnlyCollection<OrderItemDto> Items
);