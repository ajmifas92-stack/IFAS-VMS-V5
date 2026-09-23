using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using IFAS.VMS.Client.Models;
using IFAS.VMS.Client.Services;
using LibVLCSharp.Shared;

namespace IFAS.VMS.Client;

public partial class MainWindow : Window
{
    private readonly ConfigStore _store = new();
    private readonly OnvifDiscoveryService _discovery = new();
    private AppConfig _config = new();
    private LibVLC? _libVlc;
    private MediaPlayer? _player;
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
        if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(RtspBox.Text)) { MessageBox.Show("Camera name and RTSP URL are required."); return; }
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
        if (c is null) { MessageBox.Show("Select a camera first."); return; }
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
        MessageBox.Show(text, "ONVIF devices");
        StatusText.Text = $"Found {devices.Count} ONVIF device(s).";
    }

    private void OpenDataFolder_Click(object sender, RoutedEventArgs e)
    {
        var path = Selected?.RecordingPath ?? _config.DefaultRecordingRoot; Directory.CreateDirectory(path); Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = true });
    }

    private async Task ShutdownAsync()
    {
        if (_recorder != null) await _recorder.StopAsync();
        _player?.Stop(); _player?.Dispose(); _libVlc?.Dispose();
    }
}
