using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;

public class Divider : BlockBase
{
    public override string Type { get; } = "divider";
}