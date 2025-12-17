using InventoryService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Persistence;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItemEntity> InventoryItems => Set<InventoryItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItemEntity>(b =>
        {
            b.ToTable("InventoryItems");
            b.HasKey(x => x.ProductId);
            b.Property(x => x.ProductId).HasMaxLength(64);
        });
    }
}