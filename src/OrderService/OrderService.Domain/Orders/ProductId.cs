namespace OrderService.Domain.Orders;

public readonly record struct ProductId(string Value)
{
    public override string ToString() => Value.ToString();
}