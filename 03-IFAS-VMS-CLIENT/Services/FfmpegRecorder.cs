using System.Diagnostics;

namespace IFAS.VMS.Client.Services;

public sealed class FfmpegRecorder : IAsyncDisposable
{
    private Process? _process;
    public bool IsRecording => _process is { HasExited: false };

    public async Task StartAsync(string rtspUrl, string outputFile, CancellationToken ct = default)
    {
        if (IsRecording) throw new InvalidOperationException("Recording is already running.");
        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel warning -rtsp_transport tcp -i {Q(rtspUrl)} -map 0 -c copy -f segment -segment_time 300 -reset_timestamps 1 {Q(outputFile)}",
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        _process = Process.Start(psi) ?? throw new InvalidOperationException("FFmpeg could not be started. Install FFmpeg and add ffmpeg.exe to PATH.");
        await Task.Delay(250, ct);
    }

    public async Task StopAsync()
    {
        if (_process is null) return;
        try { if (!_process.HasExited) { _process.Kill(entireProcessTree: true); await _process.WaitForExitAsync(); } }
        finally { _process.Dispose(); _process = null; }
    }

    private static string Q(string value) => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    public async ValueTask DisposeAsync() => await StopAsync();
}
