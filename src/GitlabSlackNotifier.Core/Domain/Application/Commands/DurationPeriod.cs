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

    public static bool GetDurationFromString(this string input, out DurationPeriod period)
    {
        period = null;
        
        if (string.IsNullOrEmpty(input))
            return false;

        var lastChar = input[^1..];
        
        var type = lastChar.ToLower() switch
        {
            "w" => DurationType.Weeks,
            "d" => DurationType.Days,
            _ => DurationType.Unknown
        };

        if (type == DurationType.Unknown)
            return false;

        var integer = input.Substring(0, input.Length - 1);
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
