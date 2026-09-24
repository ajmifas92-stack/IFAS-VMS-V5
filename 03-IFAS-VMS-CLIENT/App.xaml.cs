using System.Windows;
using IFAS.VMS.Client.Services;

namespace IFAS.VMS.Client;

public partial class App : System.Windows.Application
{
    public ApiClient? Api { get; private set; }

    public string? Username { get; private set; }

    public string? Role { get; private set; }

    public bool IsAuthenticated =>
        Api is not null &&
        !string.IsNullOrWhiteSpace(Api.AccessToken);

    public void SetSession(
        ApiClient api,
        string username,
        string role)
    {
        Api = api;
        Username = username;
        Role = role;
    }

    public void ClearSession()
    {
        if (Api is not null)
            Api.SetAccessToken(null);

        Api = null;
        Username = null;
        Role = null;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _ = RunRecordingRetentionCleanupAsync();
        var login = new LoginWindow();
        MainWindow = login;
        login.Show();
    }
    private static async Task RunRecordingRetentionCleanupAsync()
    {
        try
        {
            var store = new ConfigStore();
            var config = await store.LoadAsync();

            var root = config.DefaultRecordingRoot;
            if (string.IsNullOrWhiteSpace(root))
                return;

            var retention = new RecordingRetentionService();
            var days = retention.GetEffectiveRetentionDays(
                config.UseCustomRetentionDays,
                config.RecordingRetentionDays,
                config.CustomRetentionDays);

            await retention.CleanupExpiredAsync(root, days);
        }
        catch
        {
            // Retention cleanup must never prevent VMS startup.
        }
    }

}
