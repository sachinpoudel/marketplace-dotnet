namespace MarketPlace.Domain.Common.Entities;

public interface IHasDomainEvents 
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}