namespace MarketPlace.Domain.Common.Entities;

public interface IAggregateRoot 
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}