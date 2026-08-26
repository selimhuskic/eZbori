using Contracts.Dtos.GeneralElections.PresidencySpecific;
using Newtonsoft.Json;

namespace External.CentralElectionCommiteeHttpClients;

public class PresidencyClient(HttpClient httpClient) : CommonCentralElectionClient, IPresidencyClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<PresidencyResultsDto[]> GetPresidentialResultsAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<PresidencyResultsDto[]>(contentString);
    }

    public async Task<PresidencyOverviewDto> GetPresidentialOverviewAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<PresidencyOverviewDto>(contentString);
    }

    public async Task<PresidencyResultsMunicipalLevelDto[]> GetPresidentialResultsMunicipalAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<PresidencyResultsMunicipalLevelDto[]>(contentString);
    }

    public async Task<PresidencyOverviewMunicipalLevelDto> GetPresidencyMunicipalOverviewAsync(string uri)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, uri).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<PresidencyOverviewMunicipalLevelDto>(contentString);
    }
}
