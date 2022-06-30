using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class SlackMessageResponse
{
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("ts")]
    public string MessageThread { get; set; }
}