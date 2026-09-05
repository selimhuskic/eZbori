using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalCouncilPartyCommand(short Year) : IRequest;

public class FetchAndStoreMunicipalCouncilPartyCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalCouncilPartyCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalCouncilPartyCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteCouncilPartiesByYearAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.LocalElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race9_electoralunitpartyresult/{cycle.ResultKey}/{municipalityCode}/1";

            var municipalityCouncilParties = await _localElectionsClient.GetMunicipalityCouncilPartiesAsync(url)
                .ConfigureAwait(false);

            var models = _mappingService.MapMunicipalityCouncilParties(municipalityCouncilParties, request.Year, municipalityCode);

            await _repository.StoreMunicipalityCouncilPartiesAsync(models).ConfigureAwait(false);
        }
    }
}
