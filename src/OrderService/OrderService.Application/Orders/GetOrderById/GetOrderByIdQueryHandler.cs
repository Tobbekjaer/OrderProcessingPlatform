using OrderService.Application.Abstractions;
using OrderService.Application.Orders.ReadModels;

namespace OrderService.Application.Orders.GetOrderById;

public sealed class GetOrderByIdQueryHandler
{
    private readonly IOrderRepository _repo;

    public GetOrderByIdQueryHandler(IOrderRepository repo) => _repo = repo;

    public async Task<OrderReadModel?> HandleAsync(GetOrderByIdQuery query, CancellationToken ct)
        => await _repo.GetReadModelByIdAsync(query.OrderId, ct);
}