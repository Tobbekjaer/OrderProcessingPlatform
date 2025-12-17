namespace InventoryService.Domain.Inventory;

public sealed class InventoryItem
{
    public ProductId ProductId { get; private set; }
    public Quantity Available { get; private set; }

    public InventoryItem(ProductId productId, Quantity initialQty)
    {
        ProductId = productId;
        Available = initialQty;
    }

    public ReservationResult Reserve(Quantity qty)
    {
        if (qty.Value <= 0)
            return ReservationResult.Invalid("Quantity must be greater than zero.");

        if (Available.Value < qty.Value)
            return ReservationResult.NotEnoughStock(Available);

        Available = Quantity.From(Available.Value - qty.Value);

        return ReservationResult.Succeeded();
    }

    public void Release(Quantity qty)
    {
        Available = Quantity.From(Available.Value + qty.Value);
    }
    
    public void AddStock(Quantity qty)
    {
        Available = Quantity.From(Available.Value + qty.Value);
    }
}