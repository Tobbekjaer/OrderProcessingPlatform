namespace Shared.Contracts.Inventory;

public sealed record ReserveInventoryRequest(
    Guid OrderId,
    IReadOnlyCollection<ReserveInventoryLineDto> Items
);