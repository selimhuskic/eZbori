using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonElectoralUnitPartyCommand(short Year) : IRequest;

public class FetchAndStoreCantonElectoralUnitPartyCommandHandler(
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    ICantonMappingService mappingService,
    ICantonClient cantonClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonElectoralUnitPartyCommand>
{
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonElectoralUnitPartyCommand request, CancellationToken cancellationToken)
    {
        var cantonCodes = _municipalityRepo.GetDistinctCantonCodes();

        await _repository.DeleteCantonElectoralUnitPartiesAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var cantonCode in cantonCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race7_electoralunitparentpartyresult/{cycle.ResultKey}/{cantonCode}/1";

            var cantonElectoralUnitParties = await _cantonClient.GetCantonElectoralUnitPartiesAsync(url)
                .ConfigureAwait(false);

            var models = _mappingService.MapCantonElectoralUnitParties(cantonElectoralUnitParties, request.Year, cantonCode);

            await _repository.StoreCantonElectoralUnitPartiesAsync(models).ConfigureAwait(false);
        }
    }
}
