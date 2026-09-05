using System.Net;
using System.Net.Mail;

namespace eZbori.Sender.Services;

public static class EmailSender
{
    public static async Task SendAsync(IConfiguration configuration, string toEmail, string toName, string subject, string htmlBody)
    {
        var smtpHost = configuration["Smtp:Host"] ?? "smtp.gmail.com";
        var port = int.TryParse(configuration["Smtp:Port"], out var p) ? p : 587;
        var user = configuration["Smtp:UserName"] ?? "";
        var pass = configuration["Smtp:Password"] ?? "";
        var fromAddr = configuration["Smtp:FromAddress"] ?? user;
        var fromName = configuration["Smtp:FromName"] ?? "eZbori";

        using var client = new SmtpClient(smtpHost, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true,
        };

        var mail = new MailMessage
        {
            From = new MailAddress(fromAddr, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        mail.To.Add(new MailAddress(toEmail, toName));
        await client.SendMailAsync(mail);
    }
}
