using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders.GetOrderById;
using OrderService.Application.Orders.GetOrders;
using OrderService.Application.Orders.PlaceOrder;
using Shared.Contracts.Orders;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly PlaceOrderCommandHandler _place;
    private readonly GetOrderByIdQueryHandler _getById;
    private readonly GetOrdersQueryHandler _getAll;

    public OrdersController(PlaceOrderCommandHandler place, GetOrderByIdQueryHandler getById, GetOrdersQueryHandler getAll)
    {
        _place = place;
        _getById = getById;
        _getAll = getAll;
    }

    [HttpPost]
    public async Task<IActionResult> Place([FromBody] PlaceOrderRequest request, CancellationToken ct)
    {
        var items = request.Items
            .Select(i => new PlaceOrderItem(i.ProductId, i.Quantity, i.UnitPrice))
            .ToList();

        var cmd = new PlaceOrderCommand(request.CustomerId, items);

        var result = await _place.HandleAsync(cmd, ct);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { message = result.Error });

        return Ok(new { orderId = result.OrderId });
    }

    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var order = await _getById.HandleAsync(new GetOrderByIdQuery(id), ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _getAll.HandleAsync(ct));
    
}
