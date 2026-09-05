using System.Text;
using System.Text.Json;
using Application.Enum;
using Application.Repositories;
using DAL.Commands.GeneralElections.Canton;
using DAL.Commands.GeneralElections.Entity;
using DAL.Commands.GeneralElections.Presidency;
using DAL.Commands.GeneralElections.State;
using DAL.Commands.LocalElections;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eZbori.Sender.Workers;

public sealed class ImportWorker(
    IConfiguration configuration,
    ILogger<ImportWorker> logger,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const string QueueName = "import.jobs";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:User"] ?? "guest",
            Password = configuration["RabbitMQ:Pass"] ?? "guest",
        };

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
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msg = JsonSerializer.Deserialize<ImportJobMessage>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (msg is null)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                await using var scope = scopeFactory.CreateAsyncScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var importJobRepo = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();

                await importJobRepo.SetRunningAsync(msg.JobId);
                logger.LogInformation("Import job {JobId} started: Year={Year} Type={Type}", msg.JobId, msg.Year, msg.ElectionType);

                try
                {
                    var year = msg.Year;
                    if (msg.ElectionType == (int)ElectionType.GeneralElection)
                    {
                        await mediator.Send(new FetchAndStorePresidencyOverviewCommand(Entity.Federation, year), stoppingToken);
                        await mediator.Send(new FetchAndStorePresidencyOverviewCommand(Entity.RS, year), stoppingToken);
                        await mediator.Send(new FetchAndStorePresidencyOverviewMunicipalLevelCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStorePresidencyResultsMunicipalLevelCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStorePresidencyResultsCommand(Constituency.Bosniak, year), stoppingToken);
                        await mediator.Send(new FetchAndStorePresidencyResultsCommand(Constituency.Croat, year), stoppingToken);
                        await mediator.Send(new FetchAndStorePresidencyResultsCommand(Constituency.Serb, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreElectoralUnitOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreElectoralUnitPartiesCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreStateMunicipalOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreStateMunicipalPartiesCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityElectoralUnitOverviewCommand(Entity.Federation, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityElectoralUnitOverviewCommand(Entity.RS, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityElectoralUnitPartiesCommand(Entity.Federation, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityElectoralUnitPartiesCommand(Entity.RS, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityPresidentOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityPresidentMunicipalResultsCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreMunicipalOverviewCommand(Entity.Federation, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreMunicipalOverviewCommand(Entity.RS, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityMunicipalPartyCommand(Entity.Federation, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreEntityMunicipalPartyCommand(Entity.RS, year), stoppingToken);
                        await mediator.Send(new FetchAndStoreCantonElectoralUnitOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreCantonElectoralUnitPartyCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreCantonMunicipalOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreCantonMunicipalPartyCommand(year), stoppingToken);
                    }
                    else
                    {
                        await mediator.Send(new FetchAndStoreMunicipalCandidateDetailsCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreMunicipalCandidateOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreMunicipalCouncilOverviewCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreMunicipalCouncilPartyCommand(year), stoppingToken);
                        await mediator.Send(new FetchAndStoreMunicipalityCouncilMinorityCommand(year), stoppingToken);
                    }

                    await importJobRepo.SetCompletedAsync(msg.JobId);
                    logger.LogInformation("Import job {JobId} completed", msg.JobId);

                    var electionLabel = msg.ElectionType == (int)ElectionType.GeneralElection ? "Opći" : "Lokalni";
                    await mediator.Send(new DAL.Commands.Notification.BroadcastNotificationCommand(
                        "Novi rezultati dostupni",
                        $"{electionLabel} izbori {msg.Year}. su sada dostupni za pregled."), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Import job {JobId} failed", msg.JobId);
                    await importJobRepo.SetFailedAsync(msg.JobId, ex.Message);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to deserialize import message");
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        logger.LogInformation("ImportWorker listening on '{Queue}'", QueueName);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private record ImportJobMessage(Guid JobId, int ElectionType, short Year);
}
