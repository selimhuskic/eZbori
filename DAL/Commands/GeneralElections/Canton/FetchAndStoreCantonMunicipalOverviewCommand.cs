using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonMunicipalOverviewCommand(short Year) : IRequest;

public class FetchAndStoreCantonMunicipalOverviewCommandHandler(
    ICantonClient cantonClient,
    ICantonMappingService mappingService,
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonMunicipalOverviewCommand>
{
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonMunicipalOverviewCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(Application.Enum.Entity.Federation);

        await _repository.DeleteCantonMunicipalOverviewAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race7_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

            var cantonMunicipalOverview = await _cantonClient.GetCantonMunicipalOverviewAsync(url)
                .ConfigureAwait(false);

            var cantonCode = _municipalityRepo.GetCantonCode(municipalityCode);

            var model = _mappingService.MapCantonMunicipalOverview(cantonMunicipalOverview, request.Year, cantonCode, municipalityCode);

            await _repository.StoreCantonMunicipalOverviewAsync(model).ConfigureAwait(false);
        }
    }
}
