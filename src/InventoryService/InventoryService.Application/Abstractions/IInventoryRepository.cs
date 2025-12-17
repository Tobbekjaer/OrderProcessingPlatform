using InventoryService.Domain.Inventory;

namespace InventoryService.Application.Abstractions;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetAsync(ProductId productId, CancellationToken ct);
    Task UpsertAsync(InventoryItem item, CancellationToken ct);
}