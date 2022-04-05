using ElsaGuides.Orders.Web.Models.Entities;

namespace ElsaGuides.Orders.Web.Contracts;

public interface IOrderService
{
    /// <summary>
    /// Creates a new order for the specified customer.
    /// </summary>
    Task<Order> CreateOrderAsync(string customerId, string description, CancellationToken cancellationToken = default);
}