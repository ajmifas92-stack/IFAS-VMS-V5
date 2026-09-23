using System.Text.Json;
using IFAS.VMS.Client.Models;

namespace IFAS.VMS.Client.Services;

public sealed class ConfigStore
{
    private readonly string _file;
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.General) { WriteIndented = true };

    public ConfigStore()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "IFAS-VMS");
        Directory.CreateDirectory(dir);
        _file = Path.Combine(dir, "client.json");
    }

    public async Task<AppConfig> LoadAsync()
    {
        if (!File.Exists(_file)) return new AppConfig();
        await using var s = File.OpenRead(_file);
        return await JsonSerializer.DeserializeAsync<AppConfig>(s, Options) ?? new AppConfig();
    }

    public async Task SaveAsync(AppConfig config)
    {
        await using var s = File.Create(_file);
        await JsonSerializer.SerializeAsync(s, config, Options);
    }

    public string FilePath => _file;
}
