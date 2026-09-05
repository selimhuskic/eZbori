using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonMunicipalPartyCommand(short Year) : IRequest;

public class FetchAndStoreCantonMunicipalPartyCommandHandler(
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    ICantonMappingService mappingService,
    ICantonClient cantonClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonMunicipalPartyCommand>
{
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonMunicipalPartyCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(Application.Enum.Entity.Federation);

        await _repository.DeleteCantonMunicipalPartiesAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race7_electoralunitpartyresult/{cycle.ResultKey}/{municipalCode}/1";

            var cantonMunicipalParties = await _cantonClient.GetCantonMunicipalPartiesAsync(url)
                .ConfigureAwait(false);

            var cantonCode = _municipalityRepo.GetCantonCode(municipalCode);

            var models = _mappingService.MapCantonMunicipalParties(cantonMunicipalParties, request.Year, cantonCode, municipalCode);

            await _repository.StoreCantonMunicipalPartiesAsync(models).ConfigureAwait(false);
        }
    }
}
