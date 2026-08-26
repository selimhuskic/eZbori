using Contracts.Dtos.GeneralElections.StateSpecific;
using Newtonsoft.Json;

namespace External.CentralElectionCommiteeHttpClients;

public class StateClient(HttpClient httpClient) : CommonCentralElectionClient, IStateClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<StateElectoralUnitOverviewDto> GetElectoralUnitOverviewAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<StateElectoralUnitOverviewDto>(contentString);
    }

    public async Task<StateElectoralUnitPartyDto[]> GetElectoralUnitPartiesAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<StateElectoralUnitPartyDto[]>(contentString);
    }

    public async Task<StateMunicipalOverviewDto> GetStateMunicipalOverviewsAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<StateMunicipalOverviewDto>(contentString);
    }

    public async Task<StateMunicipalPartyDto[]> GetStateMunicipalPartiesAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<StateMunicipalPartyDto[]>(contentString);
    }
}
