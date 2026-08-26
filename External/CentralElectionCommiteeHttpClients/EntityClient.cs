using Contracts.Dtos.GeneralElections.EntitySpecific;
using Newtonsoft.Json;

namespace External.CentralElectionCommiteeHttpClients;

public class EntityClient(HttpClient httpClient) : CommonCentralElectionClient, IEntityClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<EntityElectoralUnitOverviewDto> GetEntityElectoralUnitOverviewAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<EntityElectoralUnitOverviewDto>(contentString);
    }

    public async Task<EntityElectoralUnitPartyDto[]> GetEntityElectoralUnitPartiesAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<EntityElectoralUnitPartyDto[]>(contentString);
    }

    public async Task<EntityPresidentOverviewDto> GetEntityPresidentOverviewAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<EntityPresidentOverviewDto>(contentString);
    }

    public async Task<EntityPresidentMunicipalCandidateDto[]> GetEntityPresidentMunicipalCandidateAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<EntityPresidentMunicipalCandidateDto[]>(contentString);
    }

    public async Task<EntityMunicipalOverviewDto> GetEntityMunicipalOverviewAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<EntityMunicipalOverviewDto>(contentString);
    }

    public async Task<EntityMunicipalPartyDto[]> GetEntityMunicipalPartyAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<EntityMunicipalPartyDto[]>(contentString);
    }
}
