using GitlabSlackNotifier.Core.Domain.Slack.Application;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public class ReportPullRequestCommandModel : CommandModelBase
{
    public DurationPeriod? DurationPeriod { get; private set; }
    public DurationPeriod? SkipPeriod { get; private set; }
    
    
    [CommandProperty("channel", 
        Required = true, 
        Description = "ChannelId from slack from which it will collect data to process")]
    public string Channel { get; set; }

    [CommandProperty("duration", 
        Required = false,
        Description = "Duration in which it will collect data (1w, 4d, etc. (only w(eeks) or d(days) are supported). Default is 4w")]
    public string Duration { get; set; } = "4w";

    [CommandProperty("approvals",
        Required = false,
        Description = "How many approvals are required. Default is 2")]
    public int Approvals { get; set; } = 2;

    [CommandProperty("skip",
        Required = false,
        Description =
            "Duration in which will be skipped when processing messages data (1w, 4d, etc. (only w(eeks) or d(days) are supported). Default is `1d`")]
    public string Skip { get; set; } = "1d";
    
    [CommandProperty("output",
        Required = false,
        Description = "Output channel where result will be printed. Default is the `channel` value")]
    public string? Output { get; set; }

    public override bool IsValid()
    {
        if (!Duration.GetDurationFromString(out var durationPeriod)
            || !Skip.GetDurationFromString(out var skipPeriod))
            return false;

        DurationPeriod = durationPeriod;
        SkipPeriod = skipPeriod;

        return true;
    }
}
