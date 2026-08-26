using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonMunicipalOverviewCommand() : IRequest;

public class FetchAndStoreCantonMunicipalOverviewCommandHandler(
    ICantonClient cantonClient,
    IElectionYearsService electionYearsService,
    ICantonMappingService mappingService,
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonMunicipalOverviewCommand>
{
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonMunicipalOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(Application.Enum.Entity.Federation);

        var seededCantonMunicipalOverviewYears = await _repository
            .GetMunicipalOverviewElectionYearsAsync(municipalityCodes);

        electionYears = electionYears
            .Where(ey => !seededCantonMunicipalOverviewYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race7_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

                var cantonMunicipalOverview = await _cantonClient.GetCantonMunicipalOverviewAsync(url)
                    .ConfigureAwait(false);

                var cantonCode = _municipalityRepo.GetCantonCode(municipalityCode);

                var model = _mappingService.MapCantonMunicipalOverview(cantonMunicipalOverview, electionYear, cantonCode, municipalityCode);

                await _repository.StoreCantonMunicipalOverviewAsync(model).ConfigureAwait(false);
            }
        }
    }
}
