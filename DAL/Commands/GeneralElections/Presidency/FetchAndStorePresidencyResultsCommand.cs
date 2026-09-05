using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyResultsCommand(Constituency Constituency, short Year) : IRequest;

public class FetchAndStorePresidencyResultsCroatCommandHandler(
    IPresidencyClient centralCommissionClient,
    IPresidencyMappingService mappingService,
    IPresidencyRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyResultsCommand>
{
    private readonly IPresidencyClient _centralCommissionClient = centralCommissionClient;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IPresidencyMappingService _mappingService = mappingService;
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

        await _repository.DeletePresidencyResultsAsync(request.Year, request.Constituency);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);
        var resultsUri = $"{cycle.ApiBaseUrl}/race1_memberpresidencycandidatesresult/{cycle.ResultKey}/{constituencyCode}/1";

        var presidencyResultsDtos = await _centralCommissionClient
            .GetPresidentialResultsAsync(resultsUri);

        var presidencyResults = _mappingService.MapPresidencyResults(presidencyResultsDtos, request.Year, request.Constituency);

        await _repository.StorePresidencyResultsAsync(presidencyResults);
    }
}
