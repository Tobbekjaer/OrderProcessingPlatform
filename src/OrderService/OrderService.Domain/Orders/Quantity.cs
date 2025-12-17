namespace OrderService.Domain.Orders;

public readonly record struct Quantity(int Value)
{
    public static Quantity From(int value)
    {
        if(value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be greater than zero.");
        
        return new Quantity(value);
    }
    
    public override string ToString() => Value.ToString();
}