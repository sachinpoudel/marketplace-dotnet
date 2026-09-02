namespace MarketPlace.Domain.Common.Entities;

/// <summary>
/// Marker base for entities that are the entry point of their aggregate —
/// the only ones that get their own repository (Vendor, Product, Order, Cart, Review).
/// Child entities inside the aggregate (OrderItem, CartItem, Inventory) just
/// inherit Entity&lt;TId&gt; directly, not this.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id) { }

    protected AggregateRoot() { }
}