namespace External.CentralElectionCommiteeHttpClients;

public class CommonCentralElectionClient
{
    protected static async Task<string> FetchAndEnsureSuccess(HttpClient _httpClient, string uri)
    {
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
