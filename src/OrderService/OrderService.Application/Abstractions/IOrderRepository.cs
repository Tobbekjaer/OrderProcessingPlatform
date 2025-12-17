using OrderService.Application.Orders.ReadModels;
using OrderService.Domain.Orders;

namespace OrderService.Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<OrderReadModel?> GetReadModelByIdAsync(Guid orderId, CancellationToken ct);
    Task<IReadOnlyCollection<OrderReadModel>> GetAllReadModelsAsync(CancellationToken ct);
}