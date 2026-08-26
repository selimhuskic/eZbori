using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonElectoralUnitPartyCommand() : IRequest;

public class FetchAndStoreCantonElectoralUnitPartyCommandHandler(
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    ICantonMappingService mappingService,
    IElectionYearsService electionYearsService,
    ICantonClient cantonClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonElectoralUnitPartyCommand>
{
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonElectoralUnitPartyCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var cantonCodes = _municipalityRepo.GetDistinctCantonCodes();

        var seededCantonMunicipalPartyYears = await _repository
            .GetElectoralUnitPartyElectionYearsAsync(cantonCodes);

        electionYears = electionYears
            .Where(ey => !seededCantonMunicipalPartyYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var cantonCode in cantonCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race7_electoralunitparentpartyresult/{cycle.ResultKey}/{cantonCode}/1";

                var cantonElectoralUnitParties = await _cantonClient.GetCantonElectoralUnitPartiesAsync(url)
                    .ConfigureAwait(false);

                var models = _mappingService.MapCantonElectoralUnitParties(cantonElectoralUnitParties, electionYear, cantonCode);

                await _repository.StoreCantonElectoralUnitPartiesAsync(models).ConfigureAwait(false);
            }
        }
    }
}
