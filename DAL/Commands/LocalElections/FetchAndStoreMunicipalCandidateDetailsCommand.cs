using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalCandidateDetailsCommand() : IRequest;

public class FetchAndStoreMunicipalCandidateDetailsCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    IElectionYearsService electionYearsService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCandidateDetailsCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCandidateDetailsCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetLocalElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        var seededMunicipalCandidateDeteailsYears = await _repository
            .GetCandidateDetailsElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededMunicipalCandidateDeteailsYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.LocalElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race8_electoralunitcandidatesresult/{cycle.ResultKey}/{municipalityCode}/1";

                var municipalityCandidateDetails = await _localElectionsClient.GetMunicipalityCandidateDetailsAsync(url)
                    .ConfigureAwait(false);

                var model = _mappingService.MapMunicipalityCandidateDetails(municipalityCandidateDetails, electionYear, municipalityCode);

                await _repository.StoreMunicipalityCandidateDetailsAsync(model).ConfigureAwait(false);
            }
        }
    }
}
