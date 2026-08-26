using System.Net;
using System.Net.Mail;
using Application.Options;
using Application.Services;
using Microsoft.Extensions.Options;

namespace DAL.MappingServices;

public class SmtpEmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _opts = options.Value;

    public async Task SendAsync(string toAddress, string toName, string subject, string body)
    {
        using var client = new SmtpClient(_opts.Host, _opts.Port)
        {
            Credentials = new NetworkCredential(_opts.UserName, _opts.Password),
            EnableSsl = true,
        };

        var message = new MailMessage
        {
            From = new MailAddress(_opts.FromAddress, _opts.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        message.To.Add(new MailAddress(toAddress, toName));

        await client.SendMailAsync(message);
    }
}
