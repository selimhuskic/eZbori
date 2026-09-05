using System.Text;
using System.Text.Json;
using eZbori.Sender.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eZbori.Sender.Workers;

public sealed class InformationalEmailWorker(IConfiguration configuration, ILogger<InformationalEmailWorker> logger) : BackgroundService
{
    private const string QueueName = "user.notification";

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
                    JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

                    var payload = JsonSerializer.Deserialize<InformationalEmailPayload>(body,
                        options);

                    if (payload is not null)
                    {
                        var html = $"""
                            <p>Poštovani {payload.FirstName} {payload.LastName},</p>
                            <p>{payload.Body}</p>
                            """;

                        await EmailSender.SendAsync(configuration, payload.Email,
                            $"{payload.FirstName} {payload.LastName}",
                            payload.Subject, html);

                        logger.LogInformation("[x] Informational email sent to {Email}", payload.Email);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    acked = true;
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process informational email message (attempt {Attempt}/{Max})", attempt, maxAttempts);
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
        logger.LogInformation("InformationalEmailWorker listening on '{Queue}'", QueueName);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private record InformationalEmailPayload(string FirstName, string LastName, string Email, string Subject, string Body);
}
