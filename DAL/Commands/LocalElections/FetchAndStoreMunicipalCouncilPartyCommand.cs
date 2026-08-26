using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalCouncilPartyCommand() : IRequest;

public class FetchAndStoreMunicipalCouncilPartyCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    IElectionYearsService electionYearsService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCouncilPartyCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCouncilPartyCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetLocalElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        var seededMunicipalCouncilPartiesYears = await _repository
            .GetMunicipalCouncilPartyElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededMunicipalCouncilPartiesYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.LocalElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race9_electoralunitpartyresult/{cycle.ResultKey}/{municipalityCode}/1";

                var municipalityCouncilParties = await _localElectionsClient.GetMunicipalityCouncilPartiesAsync(url)
                    .ConfigureAwait(false);

                var models = _mappingService.MapMunicipalityCouncilParties(municipalityCouncilParties, electionYear, municipalityCode);

                await _repository.StoreMunicipalityCouncilPartiesAsync(models).ConfigureAwait(false);
            }
        }
    }
}
