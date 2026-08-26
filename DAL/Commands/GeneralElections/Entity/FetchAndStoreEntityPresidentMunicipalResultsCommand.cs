using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityPresidentMunicipalResultsCommand() : IRequest;

public class FetchAndStoreEntityPresidentMunicipalResultsCommandHandler(
    IEntityClient entityClient,
    IElectionYearsService electionYearsService,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityPresidentMunicipalResultsCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IEntityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityPresidentMunicipalResultsCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(Application.Enum.Entity.RS);

        var seededMunicipalityPartyYears = await _repository
            .GetEntityPresidentMunicipalElectionYearsAsync(municipalityCodes);

        electionYears = electionYears
            .Where(ey => !seededMunicipalityPartyYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race5_electoralunitcandidatesresult/{cycle.ResultKey}/{municipalityCode}/1";

                var entityPresidentMunicipal = await _entityClient.GetEntityPresidentMunicipalCandidateAsync(url)
                    .ConfigureAwait(false);

                var model = _mappingService.MapPresidentMunicipal(entityPresidentMunicipal, electionYear, municipalityCode);

                await _repository.StorePresidentMunicipalAsync(model).ConfigureAwait(false);
            }
        }
    }
}
