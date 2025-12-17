using InventoryService.Application.Abstractions;
using InventoryService.Domain.Inventory;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public sealed class EfInventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _db;

    public EfInventoryRepository(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task<InventoryItem?> GetAsync(ProductId productId, CancellationToken ct)
    {
        var entity = await _db.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == productId.Value, ct);

        if (entity is null) return null;

        return new InventoryItem(
            new ProductId(entity.ProductId),
            Quantity.From(entity.Available)
        );
    }

    public async Task UpsertAsync(InventoryItem item, CancellationToken ct)
    {
        var existing = await _db.InventoryItems
            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId.Value, ct);

        if (existing is null)
        {
            _db.InventoryItems.Add(new InventoryItemEntity
            {
                ProductId = item.ProductId.Value,
                Available = item.Available.Value
            });
        }
        else
        {
            existing.Available = item.Available.Value;
        }

        await _db.SaveChangesAsync(ct);
    }
}