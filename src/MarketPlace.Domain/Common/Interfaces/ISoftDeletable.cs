namespace MarketPlace.Domain.Common.Interfaces;
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedOnUtc { get; set; }
    
}