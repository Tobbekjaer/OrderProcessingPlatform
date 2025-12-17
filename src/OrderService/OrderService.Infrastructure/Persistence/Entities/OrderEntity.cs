namespace OrderService.Infrastructure.Persistence.Entities;

public sealed class OrderEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public int Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<OrderItemEntity> Items { get; set; } = new();
}