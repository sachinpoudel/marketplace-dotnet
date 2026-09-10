using System.ComponentModel.Design.Serialization;
using MediatR;

namespace MarketPlace.Domain.Common.Entities;

/// <summary>
/// Marker interface for a domain event. If you use MediatR to dispatch these,
/// make this inherit MediatR's INotification instead:
///   public interface IDomainEvent : INotification { }
/// </summary>
public interface IDomainEvent : INotification
{
}