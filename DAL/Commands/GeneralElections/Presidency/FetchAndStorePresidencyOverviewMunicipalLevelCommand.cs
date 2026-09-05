using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyOverviewMunicipalLevelCommand(short Year) : IRequest;

public class FetchAndStorePresidencyOverviewMunicipalLevelCommandHandler(
    IPresidencyClient presidencyClient,
    IMunicipalityServiceRepository municipalityRepo,
    IPresidencyRepository repository,
    IPresidencyMappingService mappingService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyOverviewMunicipalLevelCommand>
{
    private readonly IPresidencyClient _presidencyClient = presidencyClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyOverviewMunicipalLevelCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodesAndEntity = _municipalityRepo.GetAllMunicipalityCodesAndEntity();

        await _repository.DeletePresidencyMunicipalOverviewAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var (municipalityCode, entity) in municipalityCodesAndEntity)
        {
            var url = $"{cycle.ApiBaseUrl}/race1_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

            var municipalityResults = await _presidencyClient.GetPresidencyMunicipalOverviewAsync(url)
                .ConfigureAwait(false);

            var presidencyMunicipalOverview = _mappingService.MapPresidencyMunicipalOverview(municipalityResults, request.Year, entity, municipalityCode);

            await _repository.StoreOverviewAsync(presidencyMunicipalOverview).ConfigureAwait(false);
        }
    }
}
