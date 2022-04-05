using ElsaGuides.Orders.Web.Contracts;
using ElsaGuides.Orders.Web.Models.Entities;
using ElsaGuides.Orders.Web.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElsaGuides.Orders.Web.Services;

public class OrderService : IOrderService
{
    private readonly OrdersDbContext _ordersDbContext;

    public OrderService(OrdersDbContext ordersDbContext)
    {
        _ordersDbContext = ordersDbContext;
    }
    
    public async Task<Order> CreateOrderAsync(string customerId, string description, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString("N");
        var nextOrderNumber = await GetNextOrderNumberAsync(cancellationToken);
        var order = new Order(orderId, nextOrderNumber, customerId, description);

        await _ordersDbContext.Orders.AddAsync(order, cancellationToken);
        await _ordersDbContext.SaveChangesAsync(cancellationToken);

        return order;
    }

    private async Task<int> GetNextOrderNumberAsync(CancellationToken cancellationToken)
    {
        var lastNumber = await _ordersDbContext.Orders.OrderByDescending(x => x.Number).Select(x => x.Number).FirstOrDefaultAsync(cancellationToken);
        return lastNumber + 1;
    }
}