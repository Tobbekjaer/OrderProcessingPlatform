using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Application.Orders.ReadModels;
using OrderService.Domain.Orders;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Entities;

namespace OrderService.Infrastructure.Repositories;

public sealed class EfOrderRepository : IOrderRepository
{
    private readonly OrderDbContext _db;

    public EfOrderRepository(OrderDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        var entity = new OrderEntity
        {
            Id = order.Id.Value,
            CustomerId = order.CustomerId.Value,
            Status = (int)order.Status,
            TotalAmount = order.TotalAmount.value,
            CreatedAtUtc = DateTime.UtcNow,
            Items = order.Items.Select(i => new OrderItemEntity
            {
                ProductId = i.ProductId.Value,
                Quantity = i.Quantity.Value,
                UnitPrice = i.UnitPrice.value
            }).ToList()
        };

        _db.Orders.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<OrderReadModel?> GetReadModelByIdAsync(Guid orderId, CancellationToken ct)
    {
        return await _db.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new OrderReadModel(
                o.Id,
                o.CustomerId,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAtUtc,
                o.Items.Select(i => new OrderItemReadModel(
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<OrderReadModel>> GetAllReadModelsAsync(CancellationToken ct)
    {
        return await _db.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderReadModel(
                o.Id,
                o.CustomerId,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAtUtc,
                o.Items.Select(i => new OrderItemReadModel(
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice
                )).ToList()
            ))
            .ToListAsync(ct);
    }
}