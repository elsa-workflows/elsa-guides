using ElsaGuides.Orders.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElsaGuides.Orders.Web.Persistence;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; } = default!;
}