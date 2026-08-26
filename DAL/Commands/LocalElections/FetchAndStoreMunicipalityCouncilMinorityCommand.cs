using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalityCouncilMinorityCommand() : IRequest;

public class FetchAndStoreMunicipalityCouncilMinorityCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    IElectionYearsService electionYearsService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalityCouncilMinorityCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalityCouncilMinorityCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetLocalElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        var seededMunicipalCouncilMinoritiesYears = await _repository
            .GetMunicipalCouncilMinirotiesElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededMunicipalCouncilMinoritiesYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.LocalElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race9_electoralunitnationalminoritiesresult/{cycle.ResultKey}/{municipalityCode}/1";

                var municipalityCouncilMinorities = await _localElectionsClient.GetMunicipalityCouncilMinoritiesAsync(url)
                    .ConfigureAwait(false);

                var models = _mappingService.MapMunicipalityCouncilMinorities(municipalityCouncilMinorities, electionYear, municipalityCode);

                await _repository.StoreMunicipalityCouncilMinoritiesAsync(models).ConfigureAwait(false);
            }
        }
    }
}
