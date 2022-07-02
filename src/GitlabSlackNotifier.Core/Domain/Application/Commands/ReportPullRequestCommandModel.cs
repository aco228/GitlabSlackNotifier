using GitlabSlackNotifier.Core.Domain.Slack.Application;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;


public record ReportPullRequestCommandModel
{
    public DurationPeriod DurationPeriod { get; private set; }
    public DurationPeriod SkipPeriod { get; private set; }
    
    [CommandProperty("channel", Required = true)]
    public string Channel { get; set; }

    [CommandProperty("duration", Required = false)]
    public string Duration { get; set; } = "4w";

    [CommandProperty("approvals", Required = true)]
    public int Approvals { get; set; }
    
    [CommandProperty("skip", Required = true)]
    public string Skip { get; set; }

    public bool IsModelValid()
    {
        if (!Duration.GetDurationFromString(out var durationPeriod)
            || !Skip.GetDurationFromString(out var skipPeriod))
            return false;

        DurationPeriod = durationPeriod;
        SkipPeriod = skipPeriod;

        return true;
    }

    public bool GetDuration(out DurationPeriod period)
        => Duration.GetDurationFromString(out period);

    public bool GetSkipPeriod(out DurationPeriod period)
        => Skip.GetDurationFromString(out period);

    

}
