using System.IO;
using System.Windows;
using Microsoft.Win32;
using IFAS.VMS.Client.Services;

namespace IFAS.VMS.Client;

public partial class LicenseWindow : Window
{
    private App ClientApp => (App)System.Windows.Application.Current;

    private ApiClient? Api => ClientApp.Api;

    private LicenseService? _licenseService;

    public LicenseWindow()
    {
        InitializeComponent();

        if (Api is not null)
            _licenseService = new LicenseService(Api);

        Loaded += async (_, _) => await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        try
        {
            if (_licenseService is null)
            {
                StatusText.Text = "Not authenticated.";
                return;
            }

            RefreshButton.IsEnabled = false;
            StatusText.Text = "Loading license status...";
            MessageText.Text = "";

            var status = await _licenseService.GetStatusAsync();

            if (status is null)
            {
                StatusText.Text = "Unable to contact server.";
                return;
            }

            CustomerText.Text = status.Customer ?? "-";
            MaxUsersText.Text = status.MaxUsers.ToString();
            MaxCamerasText.Text = status.MaxCameras.ToString();

            CameraUsageText.Text =
                $"{status.UsedCameras} / {status.MaxCameras} " +
                $"({status.RemainingCameras} remaining)";

            ExpiryText.Text = status.ExpiresUtc.HasValue
                ? status.ExpiresUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                : "-";

            StatusText.Text = status.Valid
                ? "LICENSE VALID"
                : "LICENSE INVALID";

            MessageText.Text = status.Message ?? "";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error";
            MessageText.Text = ex.Message;
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }

    private async void Import_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_licenseService is null)
            {
                MessageText.Text = "You must be logged in.";
                return;
            }

            var dialog = new OpenFileDialog
            {
                Title = "Select IFAS VMS License",
                Filter = "IFAS License (*.ifaslic)|*.ifaslic|All Files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            var signedLicenseJson = await File.ReadAllTextAsync(
                dialog.FileName);

            if (string.IsNullOrWhiteSpace(signedLicenseJson))
            {
                MessageText.Text = "License file is empty.";
                return;
            }

            ImportButton.IsEnabled = false;
            RefreshButton.IsEnabled = false;

            StatusText.Text = "Installing license...";
            MessageText.Text = "";

            var result = await _licenseService.InstallAsync(
                signedLicenseJson);

            if (result is null)
            {
                StatusText.Text = "License installation failed.";
                MessageText.Text =
                    "Server rejected the request or is unavailable.";
                return;
            }

            CustomerText.Text = result.Customer ?? "-";
            MaxUsersText.Text = result.MaxUsers.ToString();
            MaxCamerasText.Text = result.MaxCameras.ToString();

            CameraUsageText.Text =
                $"{result.UsedCameras} / {result.MaxCameras} " +
                $"({result.RemainingCameras} remaining)";

            ExpiryText.Text = result.ExpiresUtc.HasValue
                ? result.ExpiresUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                : "-";

            StatusText.Text = result.Valid
                ? "LICENSE INSTALLED"
                : "LICENSE INVALID";

            MessageText.Text = result.Message ?? "";

            MessageBox.Show(
                this,
                result.Message ?? "License installation completed.",
                "IFAS VMS License",
                MessageBoxButton.OK,
                result.Valid
                    ? MessageBoxImage.Information
                    : MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            StatusText.Text = "Installation error.";
            MessageText.Text = ex.Message;
        }
        finally
        {
            ImportButton.IsEnabled = true;
            RefreshButton.IsEnabled = true;
        }
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
