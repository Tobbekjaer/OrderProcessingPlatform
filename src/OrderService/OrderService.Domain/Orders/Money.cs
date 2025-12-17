namespace OrderService.Domain.Orders;

public readonly record struct Money(decimal value)
{
    public static Money Zero = new(0);
    
    public static Money From(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Money cannot be negative.");

        return new Money(value);
    }
    
    public static Money operator +(Money left, Money right) =>
        new(left.value + right.value);
    
    public static Money operator *(Money left, Quantity qty) =>
        new(left.value * qty.Value);

    public override string ToString() => value.ToString("0.00");
    
}