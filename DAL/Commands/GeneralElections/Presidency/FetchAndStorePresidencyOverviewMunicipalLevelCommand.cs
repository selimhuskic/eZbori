using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyOverviewMunicipalLevelCommand() : IRequest;

public class FetchAndStorePresidencyOverviewMunicipalLevelCommandHandler(
    IPresidencyClient presidencyClient,
    IMunicipalityServiceRepository municipalityRepo,
    IPresidencyRepository repository,
    IPresidencyMappingService mappingService,
    IElectionYearsService electionYearsService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyOverviewMunicipalLevelCommand>
{
    private readonly IPresidencyClient _presidencyClient = presidencyClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyOverviewMunicipalLevelCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodesAndEntity = _municipalityRepo.GetAllMunicipalityCodesAndEntity();

        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededPresidencyMunicipalYears = await _repository
            .GetPresidencyOverviewMunicipalElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededPresidencyMunicipalYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var (municipalityCode, entity) in municipalityCodesAndEntity)
            {
                var url = $"{cycle.ApiBaseUrl}/race1_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

                var municipalityResults = await _presidencyClient.GetPresidencyMunicipalOverviewAsync(url)
                    .ConfigureAwait(false);

                var presidencyMunicipalOverview = _mappingService.MapPresidencyMunicipalOverview(municipalityResults, electionYear, entity, municipalityCode);

                await _repository.StoreOverviewAsync(presidencyMunicipalOverview).ConfigureAwait(false);
            }
        }
    }
}
