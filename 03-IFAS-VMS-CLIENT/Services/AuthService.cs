namespace IFAS.VMS.Client.Services;

public sealed record LoginRequest(
    string Username,
    string Password);

public sealed record LoginResponse(
    string AccessToken,
    int UserId,
    string Username,
    string Role);

public sealed class AuthService
{
    private readonly ApiClient _api;

    public AuthService(ApiClient api)
    {
        _api = api;
    }

    public async Task<LoginResponse?> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var request = new LoginRequest(username, password);

        var response = await _api.PostAsync<LoginRequest, LoginResponse>(
            "api/auth/login",
            request,
            cancellationToken);

        if (response is not null)
            _api.SetAccessToken(response.AccessToken);

        return response;
    }

    public void Logout()
    {
        _api.SetAccessToken(null);
    }
}
