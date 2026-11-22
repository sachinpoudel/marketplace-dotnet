namespace MarketPlace.Application.Common.Interfaces.Auth;
public interface ICurrentUser

{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    Guid GetCurrentUserId();
}
