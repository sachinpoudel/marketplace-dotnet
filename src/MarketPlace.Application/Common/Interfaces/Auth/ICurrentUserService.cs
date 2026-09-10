namespace MarketPlace.Application.Common.Interfaces.Auth;
public interface ICurrentUser

{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
