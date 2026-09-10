namespace MarketPlace.Application.Common.Interfaces.Auth;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
}