namespace IFAS.VMS.Client.Models;

public sealed class CameraEncodingSettings
{
    public string CameraName { get; set; } = string.Empty;

    public string ProfileToken { get; set; } = string.Empty;

    public int Width { get; set; }
    public int Height { get; set; }

    public int Fps { get; set; }

    public int BitrateKbps { get; set; }

    public string Codec { get; set; } = "H264";

    public string StreamType { get; set; } = "Main";

    public int Quality { get; set; }

    public bool SupportsH264 { get; set; }
    public bool SupportsH265 { get; set; }

    public bool SupportsQuality { get; set; }
    public List<string> SupportedCodecs { get; set; } = new();
    public List<string> SupportedStreamTypes { get; set; } = new();
    public List<string> SupportedResolutions { get; set; } = new();
    public int MinFps { get; set; }
    public int MaxFps { get; set; }
    public int MinBitrateKbps { get; set; }
    public int MaxBitrateKbps { get; set; }
    public int MinQuality { get; set; }
    public int MaxQuality { get; set; }

    public bool IsSupported { get; set; } = true;

    public string Message { get; set; } = string.Empty;
}
