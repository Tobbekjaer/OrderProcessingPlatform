using InventoryService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.Persistence;

public static class InventoryDbSeeder
{
    public static async Task SeedAsync(InventoryDbContext db, ILogger logger, CancellationToken ct)
    {
        // Hvis der allerede er data, gør vi ingenting
        if (await db.InventoryItems.AnyAsync(ct))
        {
            logger.LogInformation("Inventory seeding skipped (data already exists).");
            return;
        }

        logger.LogInformation("Seeding InventoryItems...");

        db.InventoryItems.AddRange(
            new InventoryItemEntity { ProductId = "P-100", Available = 10 },
            new InventoryItemEntity { ProductId = "P-200", Available = 2 },
            new InventoryItemEntity { ProductId = "P-300", Available = 0 }
        );

        await db.SaveChangesAsync(ct);

        logger.LogInformation("Inventory seeding completed.");
    }
}