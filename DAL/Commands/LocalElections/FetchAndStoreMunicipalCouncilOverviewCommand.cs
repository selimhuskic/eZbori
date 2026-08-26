using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public class FetchAndStoreMunicipalCouncilOverviewCommand() : IRequest;

public class FetchAndStoreMunicipalCouncilOverviewCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    IElectionYearsService electionYearsService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCouncilOverviewCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCouncilOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetLocalElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        var seededMunicipalCouncilOverviewYears = await _repository
            .GetMunicipalCouncilOverviewElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededMunicipalCouncilOverviewYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.LocalElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race9_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

                var municipalityCouncilParties = await _localElectionsClient.GetMunicipalityCouncilOverviewAsync(url)
                    .ConfigureAwait(false);

                if (municipalityCouncilParties == null) continue;

                var model = _mappingService.MapMunicipalityCouncilOverview(municipalityCouncilParties, electionYear, municipalityCode);

                await _repository.StoreMunicipalityCouncilOverviewAsync(model).ConfigureAwait(false);
            }
        }
    }
}
