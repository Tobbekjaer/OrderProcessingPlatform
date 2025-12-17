using OrderService.Application.Abstractions;
using OrderService.Application.Orders.ReadModels;

namespace OrderService.Application.Orders.GetOrders;

public sealed class GetOrdersQueryHandler
{
    private readonly IOrderRepository _repo;

    public GetOrdersQueryHandler(IOrderRepository repo) => _repo = repo;

    public async Task<IReadOnlyCollection<OrderReadModel>> HandleAsync(CancellationToken ct)
        => await _repo.GetAllReadModelsAsync(ct);
}