namespace OrderService.Infrastructure.Persistence.Entities;

public sealed class OrderItemEntity
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }

    public string ProductId { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public OrderEntity Order { get; set; } = default!;
}