namespace Shared.Contracts.Events;

public sealed record OrderCompletedEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount
);