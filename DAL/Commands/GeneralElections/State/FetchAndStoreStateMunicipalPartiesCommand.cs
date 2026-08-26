using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreStateMunicipalPartiesCommand() : IRequest;

public class FetchAndStoreStateMunicipalPartiesCommandHandler(
    IStateClient centralCommissionClient,
    IStateMappingService mappingService,
    IElectionYearsService electionYearsService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreStateMunicipalPartiesCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommissionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreStateMunicipalPartiesCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededElectoralUnitYears = await _repository
            .GetElectoralUnitMunicipalPartiesElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededElectoralUnitYears.Contains(ey))
            .ToArray();

        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitpartyresult/{cycle.ResultKey}/{municipalityCode}/1";

                var stateMunicipalParties = await _centralCommissionClient
                    .GetStateMunicipalPartiesAsync(uri)
                    .ConfigureAwait(false);

                var entities = _mappingService.MapStateMunicipalParties(stateMunicipalParties, electionYear, municipalityCode);

                await _repository.StoreMunicipalPartiesAsync(entities).ConfigureAwait(false);
            }
        }
    }
}
