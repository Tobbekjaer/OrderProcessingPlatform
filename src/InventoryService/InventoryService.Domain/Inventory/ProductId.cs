namespace InventoryService.Domain.Inventory;

public readonly record struct ProductId(string Value)
{
    public override string ToString() => Value.ToString();
}