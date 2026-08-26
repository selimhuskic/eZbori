using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Canton;

public record FetchAndStoreCantonMunicipalPartyCommand() : IRequest;

public class FetchAndStoreCantonMunicipalPartyCommandHandler(
    ICantonRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    ICantonMappingService mappingService,
    IElectionYearsService electionYearsService,
    ICantonClient cantonClient,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreCantonMunicipalPartyCommand>
{
    private readonly ICantonRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly ICantonMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly ICantonClient _cantonClient = cantonClient;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreCantonMunicipalPartyCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(Application.Enum.Entity.Federation);

        var seededCantonMunicipalPartyYears = await _repository
            .GetMunicipalPartyElectionYearsAsync(municipalityCodes);

        electionYears = electionYears
            .Where(ey => !seededCantonMunicipalPartyYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/race7_electoralunitpartyresult/{cycle.ResultKey}/{municipalCode}/1";

                var cantonMunicipalParties = await _cantonClient.GetCantonMunicipalPartiesAsync(url)
                    .ConfigureAwait(false);

                var cantonCode = _municipalityRepo.GetCantonCode(municipalCode);

                var models = _mappingService.MapCantonMunicipalParties(cantonMunicipalParties, electionYear, cantonCode, municipalCode);

                await _repository.StoreCantonMunicipalPartiesAsync(models).ConfigureAwait(false);
            }
        }
    }
}
