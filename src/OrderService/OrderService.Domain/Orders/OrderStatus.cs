namespace OrderService.Domain.Orders;

public enum OrderStatus
{
    Pending = 0,
    Reserved = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}