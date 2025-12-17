namespace InventoryService.Infrastructure.Persistence.Entities;

public sealed class InventoryItemEntity
{
    public string ProductId { get; set; } = default!;
    public int Available { get; set; }
}