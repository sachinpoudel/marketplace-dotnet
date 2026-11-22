using System;
using System.Net;
using System.Net.Mail;
using MarketPlace.Application.Common.Interfaces.Email;
using Microsoft.Extensions.Options;

namespace MarketPlace.Infrastructure.ExternalServices.Email;

public class SmtpEmailService(IOptions<EmailSettings> options) : IEmailService
{
    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        var client = new SmtpClient(options.Value.Host, options.Value.Port)
        {
            Credentials = new NetworkCredential(options.Value.Username, options.Value.Password),
            EnableSsl = true,
        };
        var mailMessage = new MailMessage
        {
            From = new MailAddress(options.Value.FromAddress),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(to);
        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}
