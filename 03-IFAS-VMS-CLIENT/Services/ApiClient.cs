using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IFAS.VMS.Client.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Server URL is required.", nameof(baseUrl));

        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";

        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public string? AccessToken { get; private set; }

    public void SetAccessToken(string? token)
    {
        AccessToken = token;

        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync(
            route,
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken);
    }

    public async Task<TResponse?> GetAsync<TResponse>(
        string route,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync(
            route,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken);
    }
}
