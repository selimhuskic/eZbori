using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Boundary.Messaging;

public sealed class RabbitMqPersistentConnection : IAsyncDisposable
{
    private IConnection? _connection;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IConfiguration _configuration;

    public RabbitMqPersistentConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _lock.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                UserName = _configuration["RabbitMQ:User"] ?? "guest",
                Password = _configuration["RabbitMQ:Pass"] ?? "guest",
            };

            _connection = await factory.CreateConnectionAsync();
            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
        _lock.Dispose();
    }
}
