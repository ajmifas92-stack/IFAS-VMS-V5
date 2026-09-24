using System.Windows;
using IFAS.VMS.Client.Services;

namespace IFAS.VMS.Client;

public partial class LoginWindow : Window
{
    private ApiClient? _api;
    private AuthService? _auth;

    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        var serverUrl = ServerUrlBox.Text.Trim();
        var username = UsernameBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(serverUrl))
        {
            StatusText.Text = "Server URL is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            StatusText.Text = "Username is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            StatusText.Text = "Password is required.";
            return;
        }

        try
        {
            LoginButton.IsEnabled = false;
            StatusText.Text = "Connecting to server...";

            _api = new ApiClient(serverUrl);
            _auth = new AuthService(_api);

            var result = await _auth.LoginAsync(username, password);

            if (result is null)
            {
                StatusText.Text =
                    "Invalid username/password or server unavailable.";
                return;
            }

            ((App)System.Windows.Application.Current).SetSession(
                _api,
                result.Username,
                result.Role);

            StatusText.Text =
                $"Login successful. Welcome {result.Username}.";

            var main = new MainWindow();

            System.Windows.Application.Current.MainWindow = main;

            main.Show();
            Close();
        }
        catch (Exception ex)
        {
            StatusText.Text = "Connection error: " + ex.Message;
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }
}
