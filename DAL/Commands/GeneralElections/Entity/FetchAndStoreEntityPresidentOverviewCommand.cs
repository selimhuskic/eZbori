using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityPresidentOverviewCommand(short Year) : IRequest;

public class FetchAndStoreEntityPresidentOverviewCommandHandler(
    IEntityClient entityClient,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityPresidentOverviewCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IEntityRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityPresidentOverviewCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteEntityPresidentOverviewAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);
        var url = $"{cycle.ApiBaseUrl}/race5_basicinfo/{cycle.ResultKey}";

        var entityPresidentOverview = await _entityClient.GetEntityPresidentOverviewAsync(url)
            .ConfigureAwait(false);

        var model = _mappingService.MapEntityPresidentOverview(entityPresidentOverview, request.Year, Application.Enum.Entity.RS);

        await _repository.StoreEntityPresidentOverviewAsync(model).ConfigureAwait(false);
    }
}
