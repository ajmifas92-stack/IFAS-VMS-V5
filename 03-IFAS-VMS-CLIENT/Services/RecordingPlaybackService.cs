using System.IO;

namespace IFAS.VMS.Client.Services;

public sealed record RecordingDayInfo(
    DateTime Date,
    int RecordingCount,
    bool HasAlarm);

public sealed record RecordingSegmentInfo(
    string FilePath,
    DateTime StartTime,
    DateTime EndTime);

public sealed class RecordingPlaybackService
{
    private static readonly string[] RecordingExtensions =
    {
        ".mp4",
        ".mkv",
        ".ts",
        ".avi"
    };

    public List<RecordingDayInfo> GetRecordingDays(
        string recordingRoot,
        DateTime month)
    {
        var results = new List<RecordingDayInfo>();

        if (string.IsNullOrWhiteSpace(recordingRoot) ||
            !Directory.Exists(recordingRoot))
            return results;

        var firstDay = new DateTime(month.Year, month.Month, 1);
        var lastDay = firstDay.AddMonths(1);

        foreach (var cameraDirectory in Directory.EnumerateDirectories(recordingRoot))
        {
            foreach (var dayDirectory in Directory.EnumerateDirectories(cameraDirectory))
            {
                var name = Path.GetFileName(dayDirectory);

                if (!DateTime.TryParseExact(
                        name,
                        "yyyy-MM-dd",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out var date))
                    continue;

                if (date < firstDay || date >= lastDay)
                    continue;

                var files = Directory.EnumerateFiles(
                        dayDirectory,
                        "*.*",
                        SearchOption.TopDirectoryOnly)
                    .Where(IsRecordingFile)
                    .ToList();

                if (files.Count == 0)
                    continue;

                results.Add(new RecordingDayInfo(
                    date.Date,
                    files.Count,
                    false));
            }
        }

        return results
            .GroupBy(x => x.Date)
            .Select(g => new RecordingDayInfo(
                g.Key,
                g.Sum(x => x.RecordingCount),
                g.Any(x => x.HasAlarm)))
            .OrderBy(x => x.Date)
            .ToList();
    }

    public List<RecordingSegmentInfo> GetSegments(
        string recordingRoot,
        DateTime date)
    {
        var results = new List<RecordingSegmentInfo>();

        if (string.IsNullOrWhiteSpace(recordingRoot) ||
            !Directory.Exists(recordingRoot))
            return results;

        foreach (var cameraDirectory in Directory.EnumerateDirectories(recordingRoot))
        {
            var dayDirectory = Path.Combine(
                cameraDirectory,
                date.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(dayDirectory))
                continue;

            foreach (var file in Directory.EnumerateFiles(
                         dayDirectory,
                         "*.*",
                         SearchOption.TopDirectoryOnly))
            {
                if (!IsRecordingFile(file))
                    continue;

                if (!TryGetStartTime(file, out var startTime))
                    startTime = File.GetLastWriteTime(file);

                results.Add(new RecordingSegmentInfo(
                    file,
                    startTime,
                    startTime.AddMinutes(5)));
            }
        }

        return results
            .OrderBy(x => x.StartTime)
            .ToList();
    }

    private static bool IsRecordingFile(string file)
    {
        return RecordingExtensions.Contains(
            Path.GetExtension(file),
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryGetStartTime(
        string file,
        out DateTime startTime)
    {
        startTime = default;

        var name = Path.GetFileNameWithoutExtension(file);
        var parts = name.Split('_');

        if (parts.Length < 3)
            return false;

        var date = parts[^3];
        var time = parts[^2];

        return DateTime.TryParseExact(
            date + "_" + time,
            "yyyyMMdd_HHmmss",
            null,
            System.Globalization.DateTimeStyles.None,
            out startTime);
    }
}
