namespace MarketPlace.Application.Common.Interfaces.Auth;

public interface IHttpContextProvider



{
    string? GetCurrentIpAddress();
}