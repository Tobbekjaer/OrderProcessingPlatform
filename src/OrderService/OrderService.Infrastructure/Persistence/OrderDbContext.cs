using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Persistence.Entities;

namespace OrderService.Infrastructure.Persistence;

public sealed class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>(b =>
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItemEntity>(b =>
        {
            b.ToTable("OrderItems");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductId).HasMaxLength(64);
        });
    }
}