using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalCandidateOverviewCommand(short Year) : IRequest;

public class FetchAndStoreMunicipalCandidateOverviewCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCandidateOverviewCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCandidateOverviewCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteCandidateOverviewByYearAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.LocalElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race8_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

            var municipalityCandidateDetails = await _localElectionsClient.GetMunicipalityCandidateOverviewAsync(url)
                .ConfigureAwait(false);

            if (municipalityCandidateDetails == null) continue;

            var model = _mappingService.MapMunicipalityCandidateOverview(municipalityCandidateDetails, request.Year, municipalityCode);

            await _repository.StoreMunicipalityCandidateOverviewAsync(model).ConfigureAwait(false);
        }
    }
}
