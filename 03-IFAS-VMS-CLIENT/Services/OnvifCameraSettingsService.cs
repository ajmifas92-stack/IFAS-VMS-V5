using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using IFAS.VMS.Client.Models;

namespace IFAS.VMS.Client.Services;

public sealed class OnvifCameraSettingsService
{
    private static readonly XNamespace Soap =
        "http://www.w3.org/2003/05/soap-envelope";

    private static readonly XNamespace Wsse =
        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

    private static readonly XNamespace Wsu =
        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

    private readonly HttpClient _http;

    public OnvifCameraSettingsService()
    {
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public async Task<CameraEncodingSettings> GetCurrentSettingsAsync(
        CameraProfile camera,
        CancellationToken cancellationToken = default)
    {
        if (camera is null)
            throw new ArgumentNullException(nameof(camera));

        if (string.IsNullOrWhiteSpace(camera.Host))
            throw new InvalidOperationException("Camera host/IP is required.");

        if (string.IsNullOrWhiteSpace(camera.Username))
            throw new InvalidOperationException("Camera username is required.");

        if (string.IsNullOrWhiteSpace(camera.Password))
            throw new InvalidOperationException("Camera password is required.");

        var deviceUrl = BuildDeviceServiceUrl(camera);

        var profilesResponse = await SendSoapAsync(
            deviceUrl,
            BuildGetCapabilitiesRequest(camera),
            camera.Username,
            camera.Password,
            cancellationToken);

        var mediaUrl = ExtractMediaXAddr(profilesResponse);

        if (string.IsNullOrWhiteSpace(mediaUrl))
        {
            return new CameraEncodingSettings
            {
                CameraName = camera.Name,
                IsSupported = false,
                Message = "ONVIF Media service endpoint was not found."
            };
        }

        var profiles = await SendSoapAsync(
            mediaUrl,
            BuildGetProfilesRequest(),
            camera.Username,
            camera.Password,
            cancellationToken);

        var profile = ParseFirstProfile(profiles);

        if (profile is null)
        {
            return new CameraEncodingSettings
            {
                CameraName = camera.Name,
                IsSupported = false,
                Message = "No ONVIF media profile was returned by the camera."
            };
        }

        var configuration = await SendSoapAsync(
            mediaUrl,
            BuildGetVideoEncoderConfigurationRequest(profile.Token),
            camera.Username,
            camera.Password,
            cancellationToken);

        var settings = ParseEncoderSettings(camera.Name, profile.Token, configuration);

        var optionsResponse = await SendSoapAsync(
            mediaUrl,
            BuildGetVideoEncoderConfigurationOptionsRequest(profile.Token),
            camera.Username,
            camera.Password,
            cancellationToken);

        ParseEncoderOptions(optionsResponse, settings);
        return settings;
    }

    private static string BuildDeviceServiceUrl(CameraProfile camera)
    {
        var scheme = camera.Protocol.Equals(
            "HTTPS",
            StringComparison.OrdinalIgnoreCase)
            ? "https"
            : "http";

        return $"{scheme}://{camera.Host}:{(camera.OnvifPort > 0 ? camera.OnvifPort : (scheme == "https" ? 443 : 80))}/onvif/device_service";
    }

    private async Task<XDocument> SendSoapAsync(
        string url,
        XDocument body,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var envelope = new XDocument(
            new XElement(
                Soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "s", Soap),
                new XElement(
                    Soap + "Header",
                    BuildSecurityHeader(username, password)),
                new XElement(
                    Soap + "Body",
                    body.Root?.Elements() ?? Enumerable.Empty<XElement>())));

        using var content = new StringContent(
            envelope.ToString(SaveOptions.DisableFormatting),
            Encoding.UTF8,
            "application/soap+xml");

        using var response = await _http.PostAsync(
            url,
            content,
            cancellationToken);

        var responseText = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"ONVIF request failed: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        return XDocument.Parse(responseText);
    }

    private static XElement BuildSecurityHeader(
        string username,
        string password)
    {
        var nonceBytes = RandomNumberGenerator.GetBytes(16);
        var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        var passwordDigestBytes = SHA1.HashData(
            nonceBytes
                .Concat(Encoding.UTF8.GetBytes(created))
                .Concat(Encoding.UTF8.GetBytes(password))
                .ToArray());

        var nonce = Convert.ToBase64String(nonceBytes);
        var digest = Convert.ToBase64String(passwordDigestBytes);

        return new XElement(
            Wsse + "Security",
            new XAttribute(XNamespace.Xmlns + "wsse", Wsse),
            new XAttribute(XNamespace.Xmlns + "wsu", Wsu),
            new XElement(
                Wsse + "UsernameToken",
                new XElement(
                    Wsse + "Username",
                    username),
                new XElement(
                    Wsse + "Password",
                    new XAttribute(
                        "Type",
                        "http://docs.oasis-open.org/wss/2004/01/" +
                        "oasis-200401-wss-username-token-profile-1.1#PasswordDigest"),
                    digest),
                new XElement(
                    Wsse + "Nonce",
                    new XAttribute(
                        "EncodingType",
                        "http://docs.oasis-open.org/wss/2004/01/" +
                        "oasis-200401-wss-soap-message-security-1.0#Base64Binary"),
                    nonce),
                new XElement(
                    Wsu + "Created",
                    created)));
    }

    private static XDocument BuildGetCapabilitiesRequest(CameraProfile camera)
    {
        XNamespace tds = "http://www.onvif.org/ver10/device/wsdl";

        return new XDocument(
            new XElement(
                tds + "GetCapabilities",
                new XElement(
                    tds + "Category",
                    "All")));
    }

    private static XDocument BuildGetProfilesRequest()
    {
        XNamespace trt = "http://www.onvif.org/ver10/media/wsdl";

        return new XDocument(
            new XElement(
                trt + "GetProfiles"));
    }

    private static XDocument BuildGetVideoEncoderConfigurationRequest(string profileToken)
    {
        XNamespace trt = "http://www.onvif.org/ver10/media/wsdl";

        return new XDocument(
            new XElement(
                trt + "GetVideoEncoderConfiguration",
                new XElement(
                    trt + "ConfigurationToken",
                    profileToken)));
    }

    private static string? ExtractMediaXAddr(XDocument document)
    {
        return document
            .Descendants()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "Media",
                    StringComparison.OrdinalIgnoreCase))
            ?.Value
            ?.Trim();
    }

    private static (string Token, string Name)? ParseFirstProfile(
        XDocument document)
    {
        var profile = document
            .Descendants()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "Profile",
                    StringComparison.OrdinalIgnoreCase));

        if (profile is null)
            return null;

        var token = profile
            .Attribute("token")
            ?.Value;

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var name = profile
            .Elements()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "Name",
                    StringComparison.OrdinalIgnoreCase))
            ?.Value
            ?? "Profile";

        return (token, name);
    }

    public async Task<CameraEncodingSettings> ChangeSettingsAsync(
        CameraProfile camera,
        int width,
        int height,
        int fps,
        int bitrateKbps,
        string codec,
        int quality,
        CancellationToken cancellationToken = default)
    {
        var current = await GetCurrentSettingsAsync(camera, cancellationToken);

        if (!current.IsSupported || string.IsNullOrWhiteSpace(current.ProfileToken))
            throw new InvalidOperationException(current.Message);

        var mediaUrl = await GetMediaServiceUrlAsync(camera, cancellationToken);
        var currentConfigurationResponse = await SendSoapAsync(
            mediaUrl,
            BuildGetVideoEncoderConfigurationRequest(current.ProfileToken),
            camera.Username,
            camera.Password,
            cancellationToken);

        var configuration = currentConfigurationResponse
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals("Configuration", StringComparison.OrdinalIgnoreCase));

        if (configuration is null)
            throw new InvalidOperationException("Current video encoder configuration was not returned.");

        SetElementValue(configuration, "Width", width);
        SetElementValue(configuration, "Height", height);
        SetElementValue(configuration, "FrameRateLimit", fps);
        SetElementValue(configuration, "BitrateLimit", bitrateKbps);
        SetElementValue(configuration, "Encoding", codec);

        if (quality > 0)
            SetElementValue(configuration, "Quality", quality);

        var setResponse = await SendSoapAsync(
            mediaUrl,
            BuildSetVideoEncoderConfigurationRequest(configuration),
            camera.Username,
            camera.Password,
            cancellationToken);

        _ = setResponse;

        var verified = await GetCurrentSettingsAsync(camera, cancellationToken);

        if (!verified.IsSupported)
            throw new InvalidOperationException("Camera accepted the change, but verification failed.");

        return verified;
    }

    private async Task<string> GetMediaServiceUrlAsync(
        CameraProfile camera,
        CancellationToken cancellationToken)
    {
        var deviceUrl = BuildDeviceServiceUrl(camera);
        var response = await SendSoapAsync(
            deviceUrl,
            BuildGetCapabilitiesRequest(camera),
            camera.Username,
            camera.Password,
            cancellationToken);

        var mediaUrl = ExtractMediaXAddr(response);

        if (string.IsNullOrWhiteSpace(mediaUrl))
            throw new InvalidOperationException("ONVIF Media service endpoint was not found.");

        return mediaUrl;
    }

    private static XDocument BuildSetVideoEncoderConfigurationRequest(XElement configuration)
    {
        XNamespace trt = "http://www.onvif.org/ver10/media/wsdl";

        return new XDocument(
            new XElement(
                trt + "SetVideoEncoderConfiguration",
                new XElement(
                    configuration.Name,
                    configuration.Attributes(),
                    configuration.Nodes()),
                new XElement(
                    trt + "ForcePersistence",
                    true)));
    }

    private static void SetElementValue(XElement parent, string localName, object value)
    {
        var element = parent
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase));

        if (element is not null)
            element.Value = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static XDocument BuildGetVideoEncoderConfigurationOptionsRequest(string profileToken)
    {
        XNamespace trt = "http://www.onvif.org/ver10/media/wsdl";
        return new XDocument(
            new XElement(
                trt + "GetVideoEncoderConfigurationOptions",
                new XElement(trt + "ProfileToken", profileToken)));
    }

    private static void ParseEncoderOptions(XDocument document, CameraEncodingSettings settings)
    {
        var encodings = document.Descendants()
            .Where(x => x.Name.LocalName.Equals("Encoding", StringComparison.OrdinalIgnoreCase))
            .Select(x => NormalizeCodec(x.Value.Trim()))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        settings.SupportedCodecs = encodings;
        settings.SupportsH264 = encodings.Contains("H264", StringComparer.OrdinalIgnoreCase);
        settings.SupportsH265 = encodings.Contains("H265", StringComparer.OrdinalIgnoreCase);

        var resolutions = document.Descendants()
            .Where(x => x.Name.LocalName.Equals("Resolution", StringComparison.OrdinalIgnoreCase))
            .Select(x => $"{ReadInt(x, "Width")}x{ReadInt(x, "Height")}")
            .Where(x => !x.StartsWith("0x0", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        settings.SupportedResolutions = resolutions;

        var frameRateRange = document.Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals("FrameRateRange", StringComparison.OrdinalIgnoreCase));
        var fpsRange = ReadRange(frameRateRange);
        settings.MinFps = fpsRange.Min;
        settings.MaxFps = fpsRange.Max;

        var bitrateRange = document.Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals("BitrateRange", StringComparison.OrdinalIgnoreCase));
        var bitrateValues = ReadRange(bitrateRange);
        settings.MinBitrateKbps = bitrateValues.Min;
        settings.MaxBitrateKbps = bitrateValues.Max;

        var qualityRange = document.Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals("QualityRange", StringComparison.OrdinalIgnoreCase));
        var qualityValues = ReadRange(qualityRange);
        settings.MinQuality = qualityValues.Min;
        settings.MaxQuality = qualityValues.Max;
        settings.SupportsQuality = qualityValues.Min > 0 && qualityValues.Max > 0;
    }

    private static CameraEncodingSettings ParseEncoderSettings(
        string cameraName,
        string profileToken,
        XDocument document)
    {
        var configuration = document
            .Descendants()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "Configuration",
                    StringComparison.OrdinalIgnoreCase));

        if (configuration is null)
        {
            return new CameraEncodingSettings
            {
                CameraName = cameraName,
                ProfileToken = profileToken,
                IsSupported = false,
                Message = "Video encoder configuration was not returned."
            };
        }

        var resolution = configuration
            .Descendants()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "Resolution",
                    StringComparison.OrdinalIgnoreCase));

        var width = ReadInt(resolution, "Width");
        var height = ReadInt(resolution, "Height");

        var rateControl = configuration
            .Descendants()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "RateControl",
                    StringComparison.OrdinalIgnoreCase));

        var fps = ReadInt(rateControl, "FrameRateLimit");
        var bitrate = ReadInt(rateControl, "BitrateLimit");

        var encoding = configuration
            .Elements()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    "Encoding",
                    StringComparison.OrdinalIgnoreCase))
            ?.Value
            ?? "Unknown";

        return new CameraEncodingSettings
        {
            CameraName = cameraName,
            ProfileToken = profileToken,
            Width = width,
            Height = height,
            Fps = fps,
            BitrateKbps = bitrate,
            Codec = NormalizeCodec(encoding),
            StreamType = "Main",
            SupportsH264 = encoding.Contains(
                "H264",
                StringComparison.OrdinalIgnoreCase),
            SupportsH265 = encoding.Contains(
                "H265",
                StringComparison.OrdinalIgnoreCase),
            SupportsQuality = configuration
                .Descendants()
                .Any(x =>
                    x.Name.LocalName.Equals(
                        "Quality",
                        StringComparison.OrdinalIgnoreCase)),
            IsSupported = true,
            Message = "Current ONVIF encoder settings loaded."
        };
    }

    private static (int Min, int Max) ReadRange(XElement? parent)
    {
        if (parent is null)
            return (0, 0);

        var values = parent.Descendants()
            .Where(x => x.Name.LocalName.Equals("Min", StringComparison.OrdinalIgnoreCase)
                     || x.Name.LocalName.Equals("Max", StringComparison.OrdinalIgnoreCase)
                     || x.Name.LocalName.Equals("MinValue", StringComparison.OrdinalIgnoreCase)
                     || x.Name.LocalName.Equals("MaxValue", StringComparison.OrdinalIgnoreCase))
            .Select(x => int.TryParse(x.Value, out var value) ? value : 0)
            .Where(x => x > 0)
            .ToList();

        if (values.Count < 2)
            return (0, 0);

        return (values.Min(), values.Max());
    }


    private static int ReadInt(XElement? parent, string name)
    {
        var value = parent?
            .Elements()
            .FirstOrDefault(x =>
                x.Name.LocalName.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase))
            ?.Value;

        return int.TryParse(value, out var result)
            ? result
            : 0;
    }

    private static string NormalizeCodec(string value)
    {
        if (value.Contains("H265", StringComparison.OrdinalIgnoreCase))
            return "H265";

        if (value.Contains("H264", StringComparison.OrdinalIgnoreCase))
            return "H264";

        return value;
    }
}
