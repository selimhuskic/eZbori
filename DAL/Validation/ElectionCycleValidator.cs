namespace DAL.Validation;

public static class ElectionCycleValidator
{
    public static async Task ValidateAsync(ElectionCycle cycle)
    {
        if (cycle.Year < 1990 || cycle.Year > DateTime.UtcNow.Year + 1)
            throw new UserException("Godina izbornog ciklusa nije validna.");

        InputValidator.EnsureDefinedEnum<ElectionType>(cycle.ElectionType, "ElectionType");

        if (!Uri.TryCreate(cycle.ApiBaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new UserException("API bazni URL nije ispravnog formata.");

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        try
        {
            await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new UserException("API bazni URL nije dostupan. Provjerite adresu.");
        }
    }
}
