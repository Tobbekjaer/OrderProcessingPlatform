namespace InventoryService.Domain.Inventory;

public readonly record struct Quantity(int Value)
{
    public static Quantity Zero => new(0);

    public static Quantity From(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Quantity cannot be negative.");

        return new Quantity(value);
    }

    public override string ToString() => Value.ToString();
}