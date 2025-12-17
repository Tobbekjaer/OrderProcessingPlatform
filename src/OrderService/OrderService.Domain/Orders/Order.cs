using System.Collections.ObjectModel;

namespace OrderService.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => new ReadOnlyCollection<OrderItem>(_items);

    public Money TotalAmount => _items
        .Select(i => i.TotalPrice)
        .DefaultIfEmpty(Money.Zero)
        .Aggregate(Money.Zero, (acc, next) => acc + next);

    private Order(OrderId id, CustomerId customerId)
    {
        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Pending;
    }

    public static Order CreateNew(CustomerId customerId, IEnumerable<OrderItem> items)
    {
        if (customerId.Equals(default(CustomerId)))
            throw new ArgumentException("CustomerId is required.", nameof(customerId));

        if (items is null || !items.Any())
            throw new ArgumentException("Order must contain at least one item.", nameof(items));

        var order = new Order(OrderId.New(), customerId);
        order._items.AddRange(items);
        return order;
    }

    public void MarkReserved()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be reserved.");

        Status = OrderStatus.Reserved;
    }

    public void MarkCompleted()
    {
        if (Status != OrderStatus.Reserved)
            throw new InvalidOperationException("Only reserved orders can be completed.");

        Status = OrderStatus.Completed;
    }

    public void MarkFailed()
    {
        Status = OrderStatus.Failed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Completed orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}