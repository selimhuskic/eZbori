using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public class FetchAndStoreMunicipalCandidateOverviewCommand() : IRequest;

public class FetchAndStoreMunicipalCandidateOverviewCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    IElectionYearsService electionYearsService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCandidateOverviewCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCandidateOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetLocalElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        var seededMunicipalCandidateOverviewYears = await _repository
            .GetCandidateOverviewElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededMunicipalCandidateOverviewYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.LocalElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race8_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

                var municipalityCandidateDetails = await _localElectionsClient.GetMunicipalityCandidateOverviewAsync(url)
                    .ConfigureAwait(false);

                if (municipalityCandidateDetails == null) continue;

                var model = _mappingService.MapMunicipalityCandidateOverview(municipalityCandidateDetails, electionYear, municipalityCode);

                await _repository.StoreMunicipalityCandidateOverviewAsync(model).ConfigureAwait(false);
            }
        }
    }
}
