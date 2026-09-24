using System.IO;

namespace IFAS.VMS.Client.Services;

public sealed class RecordingRetentionService
{
    private static readonly string[] RecordingExtensions =
    {
        ".mp4",
        ".mkv",
        ".ts",
        ".avi"
    };

    public int GetEffectiveRetentionDays(
        bool useCustomRetentionDays,
        int recordingRetentionDays,
        int customRetentionDays)
    {
        var days = useCustomRetentionDays
            ? customRetentionDays
            : recordingRetentionDays;

        return Math.Max(1, days);
    }

    public List<int> GetWarningDays(
        bool warning7Days,
        bool warning3Days,
        bool warning1Day,
        bool warningCustom,
        int customWarningDays)
    {
        var warnings = new List<int>();

        if (warning7Days)
            warnings.Add(7);

        if (warning3Days)
            warnings.Add(3);

        if (warning1Day)
            warnings.Add(1);

        if (warningCustom && customWarningDays > 0)
            warnings.Add(customWarningDays);

        return warnings
            .Distinct()
            .Where(x => x > 0)
            .OrderByDescending(x => x)
            .ToList();
    }

    public Task<int> CleanupExpiredAsync(
        string recordingRoot,
        int retentionDays,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(recordingRoot))
            return Task.FromResult(0);

        if (retentionDays <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(retentionDays),
                "Retention days must be greater than zero.");

        if (!Directory.Exists(recordingRoot))
            return Task.FromResult(0);

        var cutoffUtc = DateTime.UtcNow.AddDays(-retentionDays);
        var deleted = 0;

        foreach (var file in Directory.EnumerateFiles(
                     recordingRoot,
                     "*.*",
                     SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var extension = Path.GetExtension(file);

            if (!RecordingExtensions.Contains(
                    extension,
                    StringComparer.OrdinalIgnoreCase))
                continue;

            var lastWriteUtc = File.GetLastWriteTimeUtc(file);

            if (lastWriteUtc >= cutoffUtc)
                continue;

            try
            {
                File.Delete(file);
                deleted++;
            }
            catch (IOException)
            {
                // File may currently be in use by FFmpeg.
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore files that cannot currently be deleted.
            }
        }

        return Task.FromResult(deleted);
    }
}
