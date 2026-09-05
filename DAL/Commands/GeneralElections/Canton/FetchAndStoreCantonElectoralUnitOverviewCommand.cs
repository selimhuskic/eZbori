using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonElectoralUnitOverviewCommand(short Year) : IRequest;

public class FetchAndStoreCantonElectoralUnitOverviewCommandHandler(
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    ICantonMappingService mappingService,
    ICantonClient cantonClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonElectoralUnitOverviewCommand>
{
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonElectoralUnitOverviewCommand request, CancellationToken cancellationToken)
    {
        var cantonCodes = _municipalityRepo.GetDistinctCantonCodes();

        await _repository.DeleteCantonElectoralUnitOverviewAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var cantonCode in cantonCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race7_electoralunitparentbasicinfo/{cycle.ResultKey}/{cantonCode}";

            var cantonElectoralUnitOverview = await _cantonClient.GetCantonElectoralUnitOverviewAsync(url)
                .ConfigureAwait(false);

            var model = _mappingService.MapCantonElectoralUnitOverview(cantonElectoralUnitOverview, request.Year, cantonCode);

            await _repository.StoreCantonElectoralUnitOverviewAsync(model).ConfigureAwait(false);
        }
    }
}
