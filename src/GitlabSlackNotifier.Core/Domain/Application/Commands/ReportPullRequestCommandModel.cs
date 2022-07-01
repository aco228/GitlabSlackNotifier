using GitlabSlackNotifier.Core.Domain.Slack.Application;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public enum DurationType
{
    Unknown,
    Days,
    Weeks,
}

public record DurationPeriod
{
    public DateTime Now { get; set; } = DateTime.UtcNow;
    public int Value { get; set; }
    public DurationType Type { get; set; }
}

public record ReportPullRequestCommandModel
{
    [CommandProperty("channel", Required = true)]
    public string Channel { get; set; }

    [CommandProperty("duration", Required = false)]
    public string Duration { get; set; } = "4w";

    [CommandProperty("approvals", Required = false)]
    public int? Approvals { get; set; } = null;

    public bool GetDuration(out DurationPeriod period)
    {
        period = null;
        
        if (string.IsNullOrEmpty(Duration))
            return false;

        var lastChar = Duration[^1..];
        
        var type = lastChar.ToLower() switch
        {
            "w" => DurationType.Weeks,
            "d" => DurationType.Days,
            _ => DurationType.Unknown
        };

        if (type == DurationType.Unknown)
            return false;

        var integer = Duration.Substring(0, Duration.Length - 1);
        if (!int.TryParse(integer, out var intValue))
            return false;

        period = new ()
        {
            Type = type,
            Value = intValue
        };
        return true;
    }
}

public static class ReportPullRequestCommandModelExtensions
{
    public static bool IsDateInPeriod(this DurationPeriod period, DateTime dateToCompare)
    {
        var multiplier = period.Type == DurationType.Weeks ? 7 : 1;
        var currentDistance = (period.Now - dateToCompare).TotalDays;
        var expectedDistance = (period.Value * multiplier);
        var result = currentDistance < expectedDistance;
        return result;
    }

    public static int GetDayDifference(this DurationPeriod period)
    {
        var multiplier = period.Type == DurationType.Weeks ? 7 : 1;
        return period.Value * multiplier;
    }
}