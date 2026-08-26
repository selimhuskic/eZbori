using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyResultsCommand(Constituency Constituency) : IRequest;

public class FetchAndStorePresidencyResultsCroatCommandHandler(
    IPresidencyClient centralCommissionClient,
    IPresidencyMappingService mappingService,
    IPresidencyRepository repository,
    IElectionYearsService electionYearsService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyResultsCommand>
{
    private readonly IPresidencyClient _centralCommissionClient = centralCommissionClient;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyResultsCommand request, CancellationToken cancellationToken)
    {
        var constituencyCode = request.Constituency switch
        {
            Constituency.Bosniak => 701,
            Constituency.Croat => 702,
            Constituency.Serb => 703,
            _ => throw new ArgumentOutOfRangeException(nameof(request.Constituency))
        };

        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededPresidencyMunicipalYears = await _repository
            .GetPresidencyResultsElectionYearsAsync(request.Constituency);

        electionYears = electionYears
            .Where(ey => !seededPresidencyMunicipalYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);
            var resultsUri = $"{cycle.ApiBaseUrl}/race1_memberpresidencycandidatesresult/{cycle.ResultKey}/{constituencyCode}/1";

            var presidencyResultsDtos = await _centralCommissionClient
                .GetPresidentialResultsAsync(resultsUri);

            var presidencyResults = _mappingService.MapPresidencyResults(presidencyResultsDtos, electionYear, request.Constituency);

            await _repository.StorePresidencyResultsAsync(presidencyResults);
        }
    }
}
