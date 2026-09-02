namespace MarketPlace.Domain.Common.Exceptions;

/// <summary>
/// Thrown when an entity's invariant is violated (invalid state, illegal transition, etc.).
/// Fine to throw directly for now. Once you have many distinct failure cases, split into
/// subclasses (e.g. InvalidStockQuantityException : DomainException) so handlers can
/// catch specific ones — not necessary yet.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}