namespace OrderService.Domain.Orders;

public sealed class OrderItem
{
    public ProductId ProductId { get; }
    public Quantity Quantity { get; }
    public Money UnitPrice { get; }

    public Money TotalPrice => UnitPrice * Quantity;

    public OrderItem(ProductId productId, Quantity quantity, Money unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}