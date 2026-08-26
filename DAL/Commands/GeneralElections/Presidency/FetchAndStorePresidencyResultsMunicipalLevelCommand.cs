using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyResultsMunicipalLevelCommand() : IRequest;

public class FetchAndStorePresidencyResultsMunicipalLevelCommandHandler(
    IPresidencyClient presidencyClient,
    IMunicipalityServiceRepository municipalityRepo,
    IPresidencyRepository repository,
    IPresidencyMappingService mappingService,
    IElectionYearsService electionYearsService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyResultsMunicipalLevelCommand>
{
    private readonly IPresidencyClient _presidencyClient = presidencyClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyResultsMunicipalLevelCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededPresidencyMunicipalYears = await _repository
            .GetPresidencyResultsMunicipalLevelElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededPresidencyMunicipalYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race1_electoralunitcandidatesresult/{cycle.ResultKey}/{municipalityCode}/1";

                var presidentialResultsMunicipal = await _presidencyClient
                    .GetPresidentialResultsMunicipalAsync(url);

                var entity = _mappingService.MapPresidencyMunicipalResults(presidentialResultsMunicipal, electionYear, municipalityCode);

                await _repository.StorePresidencyResultsMunicipalAsync(entity);
            }
        }
    }
}
