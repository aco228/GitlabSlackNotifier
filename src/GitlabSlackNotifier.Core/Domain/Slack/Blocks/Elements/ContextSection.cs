using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;

public class ContextSection : BlockBase
{
    public override string Type { get; } = "context";

    [JsonProperty("elements")] public List<BlockBase> Elements { get; set; } = new();
}