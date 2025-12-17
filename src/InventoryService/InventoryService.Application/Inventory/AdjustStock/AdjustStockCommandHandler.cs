using InventoryService.Application.Abstractions;
using InventoryService.Domain.Inventory;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.AdjustStock;

public sealed class AdjustStockCommandHandler
{
    private readonly IInventoryRepository _repo;
    private readonly ILogger<AdjustStockCommandHandler> _logger;

    public AdjustStockCommandHandler(
        IInventoryRepository repo,
        ILogger<AdjustStockCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task HandleAsync(AdjustStockCommand cmd, CancellationToken ct)
    {
        var productId = new ProductId(cmd.ProductId);
        var qty = Quantity.From(cmd.Quantity);

        var item = await _repo.GetAsync(productId, ct)
                   ?? new InventoryItem(productId, Quantity.Zero);

        item.AddStock(qty);
        await _repo.UpsertAsync(item, ct);

        _logger.LogInformation(
            "Stock adjusted. ProductId={ProductId}, Qty={Qty}",
            productId.Value, qty.Value);
    }
}
