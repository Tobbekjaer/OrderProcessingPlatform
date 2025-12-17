using InventoryService.Application.Abstractions;
using InventoryService.Domain.Inventory;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.ReserveInventory;

public sealed class ReserveInventoryCommandHandler
{
    private readonly IInventoryRepository _repo;
    private readonly ILogger<ReserveInventoryCommandHandler> _logger;

    public ReserveInventoryCommandHandler(
        IInventoryRepository repo,
        ILogger<ReserveInventoryCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(ReserveInventoryCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Reserve inventory started. OrderId={OrderId}, Lines={Lines}",
            command.OrderId, command.Items.Count);

        foreach (var line in command.Items)
        {
            var productId = new ProductId(line.ProductId);
            var qty = Quantity.From(line.Quantity);

            _logger.LogInformation("Reserving item. OrderId={OrderId}, ProductId={ProductId}, Qty={Qty}",
                command.OrderId, productId.Value, qty.Value);

            var item = await _repo.GetAsync(productId, ct);
            if (item is null)
            {
                _logger.LogWarning("Product not found. OrderId={OrderId}, ProductId={ProductId}",
                    command.OrderId, productId.Value);
                return false;
            }

            var result = item.Reserve(qty);
            if (!result.Success)
            {
                _logger.LogWarning("Not enough stock. OrderId={OrderId}, ProductId={ProductId}, Available={Available}",
                    command.OrderId, productId.Value, result.Available?.Value);
                return false;
            }

            await _repo.UpsertAsync(item, ct);
        }

        _logger.LogInformation("Reserve inventory succeeded. OrderId={OrderId}", command.OrderId);
        return true;
    }
}