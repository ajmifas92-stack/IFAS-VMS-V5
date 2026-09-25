using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IFAS.VMS.Client.Models;
using IFAS.VMS.Client.Services;

namespace IFAS.VMS.Client;

public partial class CameraSettingsWindow : Window
{
    private readonly CameraProfile _camera;
    private readonly OnvifCameraSettingsService _service = new();
    private CameraEncodingSettings? _current;

    public CameraSettingsWindow(CameraProfile camera)
    {
        InitializeComponent();

        _camera = camera ?? throw new ArgumentNullException(nameof(camera));

        CameraTitle.Text = $"Camera Settings - {_camera.Name}";

        StreamComboBox.SelectedIndex = 0;
        ChangeButton.IsEnabled = false;

        Loaded += async (_, _) => await CheckCurrentSettingsAsync();
    }

    private async void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        await CheckCurrentSettingsAsync();
    }

    private async Task CheckCurrentSettingsAsync()
    {
        try
        {
            SetBusy(true);
            StatusBox.Text = "Connecting to camera via ONVIF...";
            SupportedBox.Text = "";

            _current = await _service.GetCurrentSettingsAsync(_camera);

            if (!_current.IsSupported)
            {
                StatusBox.Text = _current.Message;
                ChangeButton.IsEnabled = false;
                return;
            }

            ResolutionComboBox.Items.Clear();

            foreach (var resolution in _current.SupportedResolutions)
                ResolutionComboBox.Items.Add(resolution);

            if (!string.IsNullOrWhiteSpace(_current.Width.ToString()) &&
                _current.Width > 0 &&
                _current.Height > 0)
            {
                var currentResolution = $"{_current.Width}x{_current.Height}";

                if (!ResolutionComboBox.Items.Contains(currentResolution))
                    ResolutionComboBox.Items.Insert(0, currentResolution);

                ResolutionComboBox.SelectedItem = currentResolution;
            }

            CodecComboBox.Items.Clear();

            foreach (var codec in _current.SupportedCodecs)
                CodecComboBox.Items.Add(codec);

            if (!string.IsNullOrWhiteSpace(_current.Codec))
            {
                if (!CodecComboBox.Items.Contains(_current.Codec))
                    CodecComboBox.Items.Insert(0, _current.Codec);

                CodecComboBox.SelectedItem = _current.Codec;
            }

            FpsBox.Text = _current.Fps > 0
                ? _current.Fps.ToString()
                : "";

            BitrateBox.Text = _current.BitrateKbps > 0
                ? _current.BitrateKbps.ToString()
                : "";

            QualityBox.Text = _current.Quality > 0
                ? _current.Quality.ToString()
                : "";

            StatusBox.Text =
                $"Current: {_current.Width} × {_current.Height} | " +
                $"FPS: {_current.Fps} | " +
                $"Bitrate: {_current.BitrateKbps} Kbps | " +
                $"Codec: {_current.Codec}";

            SupportedBox.Text =
                $"Resolution: {FormatList(_current.SupportedResolutions)}\n" +
                $"Codec: {FormatList(_current.SupportedCodecs)}\n" +
                $"FPS: {_current.MinFps}-{_current.MaxFps}\n" +
                $"Bitrate: {_current.MinBitrateKbps}-{_current.MaxBitrateKbps} Kbps\n" +
                $"Quality: {(_current.SupportsQuality ? $"{_current.MinQuality}-{_current.MaxQuality}" : "Not reported")}";

            ChangeButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            StatusBox.Text = "Unable to read camera settings: " + ex.Message;
            ChangeButton.IsEnabled = false;
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_current is null || !_current.IsSupported)
        {
            MessageBox.Show(
                "Please check the current camera settings first.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        if (!TryReadResolution(out var width, out var height))
            return;

        if (!int.TryParse(FpsBox.Text.Trim(), out var fps) || fps <= 0)
        {
            MessageBox.Show(
                "Enter a valid FPS value.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(BitrateBox.Text.Trim(), out var bitrate) || bitrate <= 0)
        {
            MessageBox.Show(
                "Enter a valid bitrate value.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var codec = CodecComboBox.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(codec))
        {
            MessageBox.Show(
                "Select a codec.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var quality = 0;

        if (!string.IsNullOrWhiteSpace(QualityBox.Text) &&
            !int.TryParse(QualityBox.Text.Trim(), out quality))
        {
            MessageBox.Show(
                "Enter a valid quality value.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        try
        {
            SetBusy(true);
            StatusBox.Text = "Sending new settings to camera...";

            var verified = await _service.ChangeSettingsAsync(
                _camera,
                width,
                height,
                fps,
                bitrate,
                codec,
                quality);

            _current = verified;

            StatusBox.Text =
                $"Verified: {verified.Width} × {verified.Height} | " +
                $"FPS: {verified.Fps} | " +
                $"Bitrate: {verified.BitrateKbps} Kbps | " +
                $"Codec: {verified.Codec}";

            MessageBox.Show(
                "Camera settings were changed and verified successfully.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            StatusBox.Text = "Change failed: " + ex.Message;

            MessageBox.Show(
                "Camera settings could not be changed.\n\n" + ex.Message,
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private bool TryReadResolution(out int width, out int height)
    {
        width = 0;
        height = 0;

        var text = ResolutionComboBox.Text.Trim();

        var parts = text.Split(
            'x',
            'X',
            '×',
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2 ||
            !int.TryParse(parts[0].Trim(), out width) ||
            !int.TryParse(parts[1].Trim(), out height) ||
            width <= 0 ||
            height <= 0)
        {
            MessageBox.Show(
                "Enter a valid resolution, for example 1920x1080.",
                "IFAS VMS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return false;
        }

        return true;
    }

    private static string FormatList(
        System.Collections.Generic.IEnumerable<string> values)
    {
        var list = values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        return list.Count == 0
            ? "Not reported"
            : string.Join(", ", list);
    }

    private void SetBusy(bool busy)
    {
        CheckButton.IsEnabled = !busy;
        ChangeButton.IsEnabled = !busy && _current?.IsSupported == true;
        Cursor = busy
            ? System.Windows.Input.Cursors.Wait
            : System.Windows.Input.Cursors.Arrow;
    }
}
