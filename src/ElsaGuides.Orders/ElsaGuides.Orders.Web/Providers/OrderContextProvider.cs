using Elsa.Services;
using Elsa.Services.Models;
using ElsaGuides.Orders.Web.Models.Entities;
using ElsaGuides.Orders.Web.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElsaGuides.Orders.Web.Providers;

public class OrderContextProvider : WorkflowContextRefresher<Order>
{
    private readonly OrdersDbContext _ordersDbContext;

    public OrderContextProvider(OrdersDbContext ordersDbContext)
    {
        _ordersDbContext = ordersDbContext;
    }
    
    public override async ValueTask<Order?> LoadAsync(LoadWorkflowContext context, CancellationToken cancellationToken = default) => 
        await _ordersDbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.ContextId, cancellationToken);

    public override async ValueTask<string?> SaveAsync(SaveWorkflowContext<Order> context, CancellationToken cancellationToken = new CancellationToken())
    {
        await _ordersDbContext.SaveChangesAsync(cancellationToken);
        return context.ContextId;
    }
}