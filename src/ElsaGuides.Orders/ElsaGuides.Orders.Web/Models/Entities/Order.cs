using NodaTime;

namespace ElsaGuides.Orders.Web.Models.Entities;

public record Order(string Id, int Number, string CustomerId, string Description, bool Shipped = false, DateTime? ShippedAt = null);