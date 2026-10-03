namespace MarketPlace.Domain.Common.Entities;

/// <summary>
/// Marker base for entities that are the entry point of their aggregate —
/// the only ones that get their own repository (Vendor, Product, Order, Cart, Review).
/// Child entities inside the aggregate (OrderItem, CartItem, Inventory) just
/// inherit Entity&lt;TId&gt; directly, not this.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{

    private readonly List<IDomainEvent> _domainEvents = new();
      
      // Explicitly implementing the non-generic marker interface contract
      public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected AggregateRoot(TId id) : base(id) { }

    protected AggregateRoot() { }
    
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
       public void ClearDomainEvents() => _domainEvents.Clear();
}
