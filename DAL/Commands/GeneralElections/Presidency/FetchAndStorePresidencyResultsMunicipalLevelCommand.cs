using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyResultsMunicipalLevelCommand(short Year) : IRequest;

public class FetchAndStorePresidencyResultsMunicipalLevelCommandHandler(
    IPresidencyClient presidencyClient,
    IMunicipalityServiceRepository municipalityRepo,
    IPresidencyRepository repository,
    IPresidencyMappingService mappingService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyResultsMunicipalLevelCommand>
{
    private readonly IPresidencyClient _presidencyClient = presidencyClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyResultsMunicipalLevelCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeletePresidencyMunicipalResultsAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race1_electoralunitcandidatesresult/{cycle.ResultKey}/{municipalityCode}/1";

            var presidentialResultsMunicipal = await _presidencyClient
                .GetPresidentialResultsMunicipalAsync(url);

            var entity = _mappingService.MapPresidencyMunicipalResults(presidentialResultsMunicipal, request.Year, municipalityCode);

            await _repository.StorePresidencyResultsMunicipalAsync(entity);
        }
    }
}
