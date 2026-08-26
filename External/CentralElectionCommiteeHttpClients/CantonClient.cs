using Contracts.Dtos.GeneralElections.CantonSpecific;
using Newtonsoft.Json;

namespace External.CentralElectionCommiteeHttpClients;

public class CantonClient(HttpClient httpClient) : CommonCentralElectionClient, ICantonClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CantonElectoralUnitOverviewDto> GetCantonElectoralUnitOverviewAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<CantonElectoralUnitOverviewDto>(contentString);
    }

    public async Task<CantonElectoralUnitPartyDto[]> GetCantonElectoralUnitPartiesAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<CantonElectoralUnitPartyDto[]>(contentString);
    }

    public async Task<CantonMunicipalOverviewDto> GetCantonMunicipalOverviewAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<CantonMunicipalOverviewDto>(contentString);
    }

    public async Task<CantonMunicipalPartyDto[]> GetCantonMunicipalPartiesAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<CantonMunicipalPartyDto[]>(contentString);
    }
}
