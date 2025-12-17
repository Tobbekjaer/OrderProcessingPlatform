namespace InventoryService.Application.Inventory.AdjustStock;

public sealed record AdjustStockCommand(
    string ProductId,
    int Quantity
);
