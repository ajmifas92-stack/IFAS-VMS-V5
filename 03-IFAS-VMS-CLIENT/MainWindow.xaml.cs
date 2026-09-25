using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using LibVlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using IFAS.VMS.Client.Models;
using IFAS.VMS.Client.Services;
using LibVLCSharp.Shared;

namespace IFAS.VMS.Client;

public partial class MainWindow : Window
{
    private App ClientApp => (App)System.Windows.Application.Current;

    private ApiClient? Api => ClientApp.Api;

    private readonly ConfigStore _store = new();
    private readonly OnvifDiscoveryService _discovery = new();
    private readonly OnvifCameraSettingsService _cameraSettings = new();
    private AppConfig _config = new();
    private LibVLC? _libVlc;
    private LibVlcMediaPlayer? _player;
    private FfmpegRecorder? _recorder;

    public MainWindow()
    {
        InitializeComponent();
        Core.Initialize();
        _libVlc = new LibVLC("--network-caching=800", "--rtsp-tcp");
        _player = new MediaPlayer(_libVlc);
        VideoView.MediaPlayer = _player;
        Loaded += async (_, _) => await LoadAsync();
        Closed += async (_, _) => await ShutdownAsync();
    }

    private async Task LoadAsync()
    {
        _config = await _store.LoadAsync();
        if (string.IsNullOrWhiteSpace(_config.DefaultRecordingRoot))
            _config.DefaultRecordingRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "IFAS-VMS", "Recordings");
        Directory.CreateDirectory(_config.DefaultRecordingRoot);
        await CheckRecordingExpiryWarningsAsync();
        RebindList();
        StatusText.Text = $"Config: {_store.FilePath}";
    }

    private void RefreshList() => CameraList.ItemsSource = null; // selection is rebuilt below

    private void RebindList()
    {
        CameraList.ItemsSource = null;
        CameraList.ItemsSource = _config.Cameras;
    }

    private CameraProfile? Selected => CameraList.SelectedItem as CameraProfile;

    private void CameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Selected is not { } c) return;
        NameBox.Text = c.Name; RtspBox.Text = c.RtspUrl; PathBox.Text = c.RecordingPath;
    }

    private async void SaveCamera_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(RtspBox.Text)) { System.Windows.MessageBox.Show("Camera name and RTSP URL are required."); return; }
        var c = Selected ?? new CameraProfile();
        c.Name = NameBox.Text.Trim(); c.RtspUrl = RtspBox.Text.Trim(); c.RecordingPath = string.IsNullOrWhiteSpace(PathBox.Text) ? Path.Combine(_config.DefaultRecordingRoot, c.Name) : PathBox.Text.Trim();
        Directory.CreateDirectory(c.RecordingPath);
        if (!_config.Cameras.Contains(c)) _config.Cameras.Add(c);
        await _store.SaveAsync(_config); RebindList(); CameraList.SelectedItem = c; StatusText.Text = "Camera saved.";
    }

    private async void DeleteCamera_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is not { } c) return;
        _config.Cameras.Remove(c); await _store.SaveAsync(_config); RebindList(); StatusText.Text = "Camera deleted.";
    }

    private async void Connect_Click(object sender, RoutedEventArgs e)
    {
        var c = Selected;
        if (c is null) { System.Windows.MessageBox.Show("Select a camera first."); return; }
        try
        {
            _player!.Stop();
            using var media = new Media(_libVlc!, new Uri(c.RtspUrl));
            _player.Play(media);
            StatusText.Text = $"LIVE: {c.Name}";
        }
        catch (Exception ex) { StatusText.Text = "Live view error: " + ex.Message; }
    }

    private void Stop_Click(object sender, RoutedEventArgs e) { _player?.Stop(); StatusText.Text = "Stopped."; }

    private async void Record_Click(object sender, RoutedEventArgs e)
    {
        var c = Selected; if (c is null) return;
        try
        {
            _recorder ??= new FfmpegRecorder();
            var day = Path.Combine(c.RecordingPath, DateTime.Now.ToString("yyyy-MM-dd")); Directory.CreateDirectory(day);
            var file = Path.Combine(day, $"{c.Name}_{DateTime.Now:yyyyMMdd_HHmmss}_%03d.mp4");
            await _recorder.StartAsync(c.RtspUrl, file);
            StatusText.Text = "Recording started: " + day;
        }
        catch (Exception ex) { StatusText.Text = "Recording error: " + ex.Message; }
    }

    private async void StopRecord_Click(object sender, RoutedEventArgs e) { if (_recorder != null) await _recorder.StopAsync(); StatusText.Text = "Recording stopped."; }

    private async void Snapshot_Click(object sender, RoutedEventArgs e)
    {
        var c = Selected; if (c is null) return;
        var dir = Path.Combine(c.RecordingPath, "Snapshots"); Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, $"{c.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
        var psi = new ProcessStartInfo("ffmpeg", $"-y -rtsp_transport tcp -i \"{c.RtspUrl}\" -frames:v 1 \"{file}\"") { UseShellExecute = false, CreateNoWindow = true };
        try { using var p = Process.Start(psi)!; await p.WaitForExitAsync(); StatusText.Text = "Snapshot: " + file; }
        catch (Exception ex) { StatusText.Text = "Snapshot error: " + ex.Message; }
    }

    private void BrowsePath_Click(object sender, RoutedEventArgs e)
    {
        using var d = new Forms.FolderBrowserDialog { Description = "Select recording folder" };
        if (d.ShowDialog() == Forms.DialogResult.OK) PathBox.Text = d.SelectedPath;
    }

    private async void Discover_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Searching ONVIF devices...";
        var devices = await _discovery.DiscoverAsync(TimeSpan.FromSeconds(4));
        if (devices.Count == 0) { StatusText.Text = "No ONVIF devices found."; return; }
        var text = string.Join(Environment.NewLine, devices.Select(x => x.Address));
        System.Windows.MessageBox.Show(text, "ONVIF devices");
        StatusText.Text = $"Found {devices.Count} ONVIF device(s).";
    }

    private void CameraManagement_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null)
        {
            System.Windows.MessageBox.Show("Please select a camera first.", "IFAS VMS", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return;
        }

        var window = new CameraSettingsWindow(Selected)
        {
            Owner = this
        };

        window.ShowDialog();
    }

    private void OpenDataFolder_Click(object sender, RoutedEventArgs e)
    {
        var path = Selected?.RecordingPath ?? _config.DefaultRecordingRoot; Directory.CreateDirectory(path); Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = true });
    }

    private void License_Click(object sender, RoutedEventArgs e)
    {
        var licenseWindow = new LicenseWindow
        {
            Owner = this
        };

        licenseWindow.ShowDialog();
    }

    private async Task ShutdownAsync()
    {
        if (_recorder != null) await _recorder.StopAsync();
        _player?.Stop(); _player?.Dispose(); _libVlc?.Dispose();
    }
    private async Task CheckRecordingExpiryWarningsAsync()
    {
        try
        {
            var retention = new RecordingRetentionService();
            var days = retention.GetEffectiveRetentionDays(
                _config.UseCustomRetentionDays,
                _config.RecordingRetentionDays,
                _config.CustomRetentionDays);

            var warningDays = retention.GetWarningDays(
                _config.RetentionWarning7Days,
                _config.RetentionWarning3Days,
                _config.RetentionWarning1Day,
                _config.RetentionWarningCustom,
                _config.CustomRetentionWarningDays);

            var warningService = new RecordingExpiryWarningService();
            var warnings = warningService.FindWarnings(
                _config.DefaultRecordingRoot,
                days,
                warningDays);

            if (warnings.Count == 0)
                return;

            var nearest = warnings
                .OrderBy(x => x.ExpiryDateUtc)
                .First();

            StatusText.Text =
                $"Recording expiry warning: {warnings.Count} file(s), nearest expiry in {nearest.RemainingDays} day(s).";

            System.Windows.MessageBox.Show(
                $"Recording retention warning.\n\n{warnings.Count} recording file(s) are approaching expiry.\nNearest expiry: {nearest.ExpiryDateUtc.ToLocalTime():yyyy-MM-dd HH:mm}",
                "IFAS VMS - Recording Retention",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            StatusText.Text = "Retention warning error: " + ex.Message;
        }
    }

    private void Playback_Click(object sender, RoutedEventArgs e)
    {
        PlaybackPanel.Visibility = Visibility.Visible;
        _playbackMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        PlaybackCalendar.DisplayDate = _playbackMonth;
        StatusText.Text = "Playback calendar loaded.";
    }

    private readonly RecordingPlaybackService _playbackService = new();
    private DateTime _playbackMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    private void ClosePlayback_Click(object sender, RoutedEventArgs e)
    {
        PlaybackPanel.Visibility = Visibility.Collapsed;
    }

    private void PlaybackPreviousMonth_Click(object sender, RoutedEventArgs e)
    {
        _playbackMonth = _playbackMonth.AddMonths(-1);
        PlaybackCalendar.DisplayDate = _playbackMonth;
    }

    private void PlaybackNextMonth_Click(object sender, RoutedEventArgs e)
    {
        _playbackMonth = _playbackMonth.AddMonths(1);
        PlaybackCalendar.DisplayDate = _playbackMonth;
    }

    private void PlaybackCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
    {
        _playbackMonth = new DateTime(PlaybackCalendar.DisplayDate.Year, PlaybackCalendar.DisplayDate.Month, 1);
        LoadPlaybackMonth();
    }

    private void PlaybackCalendar_Loaded(object sender, RoutedEventArgs e)
    {
        LoadPlaybackMonth();
    }

    private void PlaybackCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PlaybackCalendar.SelectedDate is not DateTime date)
            return;

        var segments = _playbackService.GetSegments(_config.DefaultRecordingRoot, date);
        PlaybackTimeline.ItemsSource = segments;
        StatusText.Text = segments.Count == 0
            ? $"No recordings for {date:yyyy-MM-dd}."
            : $"{segments.Count} recording segment(s) for {date:yyyy-MM-dd}.";
    }

    private void LoadPlaybackMonth()
    {
        if (PlaybackMonthText == null || PlaybackCalendar == null)
            return;

        PlaybackMonthText.Text = _playbackMonth.ToString("MMMM yyyy");

        _playbackRecordingDates.Clear();

        var days = _playbackService.GetRecordingDays(
            _config.DefaultRecordingRoot,
            _playbackMonth);

        foreach (var day in days)
            _playbackRecordingDates.Add(day.Date.Date);

        RefreshPlaybackCalendarVisuals();
    }

    private void RefreshPlaybackCalendarVisuals()
    {
        if (PlaybackCalendar == null)
            return;

        UpdateLayout();
        PlaybackCalendar.UpdateLayout();

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(PlaybackCalendar); i++)
        {
            if (VisualTreeHelper.GetChild(PlaybackCalendar, i) is DependencyObject child)
                ApplyPlaybackDayColors(child);
        }
    }

    private void ApplyPlaybackDayColors(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is CalendarDayButton dayButton && dayButton.DataContext is DateTime date)
            {
                if (dayButton.IsSelected)
                    dayButton.Background = new SolidColorBrush(Color.FromRgb(36, 119, 200));
                else if (_playbackRecordingDates.Contains(date.Date))
                    dayButton.Background = new SolidColorBrush(Color.FromRgb(30, 142, 74));
            }

            ApplyPlaybackDayColors(child);
        }
    }
}
