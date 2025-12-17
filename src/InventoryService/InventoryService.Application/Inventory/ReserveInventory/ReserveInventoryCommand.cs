namespace InventoryService.Application.Inventory.ReserveInventory;

public sealed record ReserveInventoryCommand(
    Guid OrderId,
    IReadOnlyCollection<ReserveInventoryLine> Items
);

public sealed record ReserveInventoryLine(
    string ProductId,
    int Quantity
);