using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.LocalElections;

public record FetchAndStoreMunicipalityCouncilMinorityCommand(short Year) : IRequest;

public class FetchAndStoreMunicipalityCouncilMinorityCommandHandler(
    IMunicipalityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IMunicipalityMappingService mappingService,
    ILocalElectionsClient localElectionsClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalityCouncilMinorityCommand>
{
    private readonly IMunicipalityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IMunicipalityMappingService _mappingService = mappingService;
    private readonly ILocalElectionsClient _localElectionsClient = localElectionsClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalityCouncilMinorityCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteCouncilMinoritiesByYearAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.LocalElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race9_electoralunitnationalminoritiesresult/{cycle.ResultKey}/{municipalityCode}/1";

            var municipalityCouncilMinorities = await _localElectionsClient.GetMunicipalityCouncilMinoritiesAsync(url)
                .ConfigureAwait(false);

            var models = _mappingService.MapMunicipalityCouncilMinorities(municipalityCouncilMinorities, request.Year, municipalityCode);

            await _repository.StoreMunicipalityCouncilMinoritiesAsync(models).ConfigureAwait(false);
        }
    }
}
