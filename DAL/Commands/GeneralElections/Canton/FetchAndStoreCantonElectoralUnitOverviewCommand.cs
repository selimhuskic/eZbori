using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonElectoralUnitOverviewCommand() : IRequest;

public class FetchAndStoreCantonElectoralUnitOverviewCommandHandler(
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    ICantonMappingService mappingService,
    IElectionYearsService electionYearsService,
    ICantonClient cantonClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonElectoralUnitOverviewCommand>
{
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonElectoralUnitOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var cantonCodes = _municipalityRepo.GetDistinctCantonCodes();

        var seededCantonElectoralUnitYears = await _repository
            .GetElectoralUnitOverviewElectionYearsAsync(cantonCodes);

        electionYears = electionYears
            .Where(ey => !seededCantonElectoralUnitYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var cantonCode in cantonCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race7_electoralunitparentbasicinfo/{cycle.ResultKey}/{cantonCode}";

                var cantonElectoralUnitOverview = await _cantonClient.GetCantonElectoralUnitOverviewAsync(url)
                    .ConfigureAwait(false);

                var model = _mappingService.MapCantonElectoralUnitOverview(cantonElectoralUnitOverview, electionYear, cantonCode);

                await _repository.StoreCantonElectoralUnitOverviewAsync(model).ConfigureAwait(false);
            }
        }
    }
}
