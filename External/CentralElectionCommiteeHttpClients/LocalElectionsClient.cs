using Contracts.Dtos.LocalElections.CandidateSpecific;
using Contracts.Dtos.LocalElections.CouncilSpecific;
using Newtonsoft.Json;

namespace External.CentralElectionCommiteeHttpClients;

public class LocalElectionsClient(HttpClient httpClient) : CommonCentralElectionClient, ILocalElectionsClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<MunicipalityCandidateDetailsDto>> GetMunicipalityCandidateDetailsAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<IEnumerable<MunicipalityCandidateDetailsDto>>(contentString);
    }

    public async Task<MunicipalityCandidateOverviewDto?> GetMunicipalityCandidateOverviewAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<MunicipalityCandidateOverviewDto?>(contentString);
    }

    public async Task<MunicipalityCouncilOverviewDto?> GetMunicipalityCouncilOverviewAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<MunicipalityCouncilOverviewDto?>(contentString);
    }

    public async Task<IEnumerable<MunicipalityCouncilPartyDto>> GetMunicipalityCouncilPartiesAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<IEnumerable<MunicipalityCouncilPartyDto>>(contentString);
    }

    public async Task<IEnumerable<MunicipalityCouncilMinorityDto>> GetMunicipalityCouncilMinoritiesAsync(string url)
    {
        var contentString = await FetchAndEnsureSuccess(_httpClient, url).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<IEnumerable<MunicipalityCouncilMinorityDto>>(contentString);
    }
}
