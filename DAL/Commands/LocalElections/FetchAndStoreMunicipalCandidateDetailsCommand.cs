using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalCandidateDetailsCommand(short Year) : IRequest;

public class FetchAndStoreMunicipalCandidateDetailsCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCandidateDetailsCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCandidateDetailsCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteCandidateDetailsByYearAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.LocalElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race8_electoralunitcandidatesresult/{cycle.ResultKey}/{municipalityCode}/1";

            var municipalityCandidateDetails = await _localElectionsClient.GetMunicipalityCandidateDetailsAsync(url)
                .ConfigureAwait(false);

            var model = _mappingService.MapMunicipalityCandidateDetails(municipalityCandidateDetails, request.Year, municipalityCode);

            await _repository.StoreMunicipalityCandidateDetailsAsync(model).ConfigureAwait(false);
        }
    }
}
