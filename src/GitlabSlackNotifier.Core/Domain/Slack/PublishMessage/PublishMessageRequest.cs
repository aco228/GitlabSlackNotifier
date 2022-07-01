using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack;

public record PublishMessageRequest
{
    [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
    public string ChannelId { get; set; }
    
    [JsonProperty("thread_ts", NullValueHandling = NullValueHandling.Ignore)]
    public string Thread { get; set; }
    
    [JsonProperty("text")]
    public string Message { get; set; }
    
    [JsonProperty("link_names")]
    public string IncludeMentions { get; set; } = "1";
    
    [JsonProperty("unfurl_links")]
    public bool UnfurLinks { get; set; } = false;
    
    [JsonProperty("blocks", NullValueHandling = NullValueHandling.Ignore)]
    public List<BlockBase> Blocks { get; set; }
}