using System.Text;
using System.Text.Json;
using Application.Models;
using Application.Services;
using RabbitMQ.Client;

namespace Boundary.Messaging;

public class RabbitMqPasswordResetQueue(RabbitMqPersistentConnection persistentConnection) : IPasswordResetQueue
{
    private const string QueueName = "user.password_reset";

    public async Task PublishAsync(PasswordResetMessage message)
    {
        var connection = await persistentConnection.GetConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(message);
        ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties { Persistent = true };
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueName, mandatory: false, basicProperties: props, body: body);
    }
}
