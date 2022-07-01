using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

public abstract class BlockBase
{
    [JsonProperty("type")]
    public abstract string Type { get; }
        
    [JsonProperty("block_id", NullValueHandling = NullValueHandling.Ignore)]
    public string BlockId { get; set; }
}