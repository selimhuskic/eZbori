using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<InviteEmailWorker>();
builder.Services.AddHostedService<PasswordResetEmailWorker>();
var host = builder.Build();
await host.RunAsync();


sealed class InviteEmailWorker(IConfiguration configuration, ILogger<InviteEmailWorker> logger) : BackgroundService
{
    private const string QueueName = "user.invited";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:User"] ?? "guest",
            Password = configuration["RabbitMQ:Pass"] ?? "guest",
        };

        logger.LogInformation("Connecting to RabbitMQ at {Host}", factory.HostName);

        IConnection? connection = null;
        var delay = TimeSpan.FromSeconds(1);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                connection = await factory.CreateConnectionAsync(stoppingToken);
                break;
            }
            catch (Exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("RabbitMQ not ready, retrying in {Delay}s...", delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 30));
            }
        }
        if (connection is null) return;
        await using var _ = connection;
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: QueueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            const int maxAttempts = 4;
            var retryDelay = TimeSpan.FromSeconds(1);
            bool acked = false;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var payload = JsonSerializer.Deserialize<InvitePayload>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (payload is not null)
                    {
                        var customPart = string.IsNullOrWhiteSpace(payload.CustomMessage)
                            ? string.Empty
                            : $"<p>{payload.CustomMessage}</p>";

                        var html = $"""
                            <p>Poštovani {payload.FirstName} {payload.LastName},</p>
                            <p>Pozvani ste na platformu <strong>eZbori</strong>.</p>
                            {customPart}
                            <p>Prijavite se koristeći svoju email adresu kao korisničko ime.</p>
                            <p>Vaša privremena lozinka je: <strong>{payload.TempPassword}</strong></p>
                            <p>Bit ćete preusmjereni na promjenu lozinke pri prvoj prijavi.</p>
                            """;

                        await EmailSender.SendAsync(configuration, payload.Email,
                            $"{payload.FirstName} {payload.LastName}",
                            "Pozivnica — eZbori platforma", html);

                        logger.LogInformation("[x] Invite email sent to {Email}", payload.Email);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    acked = true;
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process invite message (attempt {Attempt}/{Max})", attempt, maxAttempts);
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(retryDelay, stoppingToken);
                        retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, 8));
                    }
                }
            }
            if (!acked)
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
        };

        await channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        logger.LogInformation("InviteEmailWorker listening on '{Queue}'", QueueName);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private record InvitePayload(string FirstName, string LastName, string Email, string? CustomMessage, string? TempPassword);
}

sealed class PasswordResetEmailWorker(IConfiguration configuration, ILogger<PasswordResetEmailWorker> logger) : BackgroundService
{
    private const string QueueName = "user.password_reset";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:User"] ?? "guest",
            Password = configuration["RabbitMQ:Pass"] ?? "guest",
        };

        IConnection? connection2 = null;
        var delay2 = TimeSpan.FromSeconds(1);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                connection2 = await factory.CreateConnectionAsync(stoppingToken);
                break;
            }
            catch (Exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("RabbitMQ not ready, retrying in {Delay}s...", delay2.TotalSeconds);
                await Task.Delay(delay2, stoppingToken);
                delay2 = TimeSpan.FromSeconds(Math.Min(delay2.TotalSeconds * 2, 30));
            }
        }
        if (connection2 is null) return;
        await using var _2 = connection2;
        await using var channel = await connection2.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: QueueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            const int maxAttempts = 4;
            var retryDelay = TimeSpan.FromSeconds(1);
            bool acked = false;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var payload = JsonSerializer.Deserialize<ResetPayload>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (payload is not null)
                    {
                        var html = $"""
                            <p>Poštovani {payload.FirstName},</p>
                            <p>Primili smo zahtjev za resetovanje lozinke za vaš eZbori nalog.</p>
                            <p>Vaš kod za resetovanje lozinke je: <strong>{payload.Token}</strong></p>
                            <p>Kod je važeći 30 minuta. Ako niste vi zatražili resetovanje, ignorišite ovaj email.</p>
                            """;

                        await EmailSender.SendAsync(configuration, payload.Email, payload.FirstName,
                            "Resetovanje lozinke — eZbori", html);

                        logger.LogInformation("[x] Password reset email sent to {Email}", payload.Email);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    acked = true;
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process password reset message (attempt {Attempt}/{Max})", attempt, maxAttempts);
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(retryDelay, stoppingToken);
                        retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, 8));
                    }
                }
            }
            if (!acked)
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
        };

        await channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        logger.LogInformation("PasswordResetEmailWorker listening on '{Queue}'", QueueName);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private record ResetPayload(string FirstName, string Email, string Token);
}

static class EmailSender
{
    internal static async Task SendAsync(IConfiguration configuration, string toEmail, string toName, string subject, string htmlBody)
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
