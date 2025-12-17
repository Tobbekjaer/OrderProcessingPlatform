using Shared.Contracts.Inventory;

namespace OrderService.Application.Abstractions;

public interface IInventoryClient
{
    Task<bool> ReserveAsync(Guid orderId, IReadOnlyCollection<ReserveInventoryLineDto> items, CancellationToken ct);
}