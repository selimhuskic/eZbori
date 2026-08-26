using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityPresidentOverviewCommand() : IRequest;

public class FetchAndStoreEntityPresidentOverviewCommandHandler(
    IEntityClient entityClient,
    IElectionYearsService electionYearsService,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityPresidentOverviewCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IEntityRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityPresidentOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededMunicipalityPartyYears = await _repository
           .GetEntityPresidentOverviewElectionYearsAsync(Application.Enum.Entity.RS);

        electionYears = electionYears
            .Where(ey => !seededMunicipalityPartyYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);
            var url = $"{cycle.ApiBaseUrl}/race5_basicinfo/{cycle.ResultKey}";

            var entityPresidentOverview = await _entityClient.GetEntityPresidentOverviewAsync(url)
                .ConfigureAwait(false);

            var model = _mappingService.MapEntityPresidentOverview(entityPresidentOverview, electionYear, Application.Enum.Entity.RS);

            await _repository.StoreEntityPresidentOverviewAsync(model).ConfigureAwait(false);
        }
    }
}
