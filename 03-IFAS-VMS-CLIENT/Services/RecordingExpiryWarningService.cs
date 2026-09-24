using System.IO;

namespace IFAS.VMS.Client.Services;

public sealed record RecordingExpiryWarning(
    string FilePath,
    DateTime RecordingDateUtc,
    DateTime ExpiryDateUtc,
    int RemainingDays);

public sealed class RecordingExpiryWarningService
{
    private static readonly string[] RecordingExtensions =
    {
        ".mp4",
        ".mkv",
        ".ts",
        ".avi"
    };

    public List<RecordingExpiryWarning> FindWarnings(
        string recordingRoot,
        int retentionDays,
        IEnumerable<int> warningDays)
    {
        var results = new List<RecordingExpiryWarning>();

        if (string.IsNullOrWhiteSpace(recordingRoot))
            return results;

        if (retentionDays <= 0 || !Directory.Exists(recordingRoot))
            return results;

        var thresholds = warningDays
            .Where(x => x > 0)
            .Distinct()
            .OrderByDescending(x => x)
            .ToList();

        if (thresholds.Count == 0)
            return results;

        var nowUtc = DateTime.UtcNow;

        foreach (var file in Directory.EnumerateFiles(
                     recordingRoot,
                     "*.*",
                     SearchOption.AllDirectories))
        {
            var extension = Path.GetExtension(file);

            if (!RecordingExtensions.Contains(
                    extension,
                    StringComparer.OrdinalIgnoreCase))
                continue;

            DateTime recordingDateUtc;

            try
            {
                recordingDateUtc = File.GetLastWriteTimeUtc(file);
            }
            catch
            {
                continue;
            }

            var expiryDateUtc = recordingDateUtc.AddDays(retentionDays);

            if (expiryDateUtc <= nowUtc)
                continue;

            var remainingDays =
                Math.Max(
                    0,
                    (int)Math.Ceiling(
                        (expiryDateUtc - nowUtc).TotalDays));

            if (thresholds.Any(x => remainingDays <= x))
            {
                results.Add(new RecordingExpiryWarning(
                    file,
                    recordingDateUtc,
                    expiryDateUtc,
                    remainingDays));
            }
        }

        return results
            .OrderBy(x => x.ExpiryDateUtc)
            .ToList();
    }
}
