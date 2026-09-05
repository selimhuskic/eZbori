using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalCouncilOverviewCommand(short Year) : IRequest;

public class FetchAndStoreMunicipalCouncilOverviewCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCouncilOverviewCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCouncilOverviewCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteCouncilOverviewByYearAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.LocalElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race9_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

            var municipalityCouncilParties = await _localElectionsClient.GetMunicipalityCouncilOverviewAsync(url)
                .ConfigureAwait(false);

            if (municipalityCouncilParties == null) continue;

            var model = _mappingService.MapMunicipalityCouncilOverview(municipalityCouncilParties, request.Year, municipalityCode);

            await _repository.StoreMunicipalityCouncilOverviewAsync(model).ConfigureAwait(false);
        }
    }
}
