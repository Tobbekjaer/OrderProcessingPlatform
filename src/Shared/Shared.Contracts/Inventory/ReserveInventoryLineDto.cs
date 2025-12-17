namespace Shared.Contracts.Inventory;

public sealed record ReserveInventoryLineDto(
    string ProductId,
    int Quantity
);