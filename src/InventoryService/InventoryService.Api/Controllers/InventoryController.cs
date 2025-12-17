using InventoryService.Application.Inventory.AdjustStock;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Application.Inventory.ReserveInventory;
using Shared.Contracts.Inventory;
using Shared.Contracts.Orders;

namespace InventoryService.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly ReserveInventoryCommandHandler _reserveInventoryHandler;
    private readonly AdjustStockCommandHandler _adjustStockHandler;

    public InventoryController(ReserveInventoryCommandHandler reserveInventoryHandler, AdjustStockCommandHandler adjustStockHandler)
    {
        _reserveInventoryHandler = reserveInventoryHandler;
        _adjustStockHandler = adjustStockHandler;
    }

    [HttpPost("reservations")]
    public async Task<IActionResult> Reserve([FromBody] ReserveInventoryRequest request, CancellationToken ct)
    {
        var lines = request.Items
            .Select(i => new ReserveInventoryLine(i.ProductId, i.Quantity))
            .ToList();

        var cmd = new ReserveInventoryCommand(request.OrderId, lines);

        var ok = await _reserveInventoryHandler.HandleAsync(cmd, ct);
        return ok ? Ok() : Conflict(new { message = "Not enough stock" });
    }
    
    [HttpPost("adjustments")]
    public async Task<IActionResult> Adjust([FromBody] AdjustStockCommand cmd, CancellationToken ct)
    {
        await _adjustStockHandler.HandleAsync(cmd, ct);
        return Ok();
    }

}