using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class ConversationMessagesRequest
{
    [JsonProperty("limit")]
    public int Limit { get; set; } = 100;
    
    [JsonProperty("channel")]
    public string Channel { get; set; }
    
    [JsonProperty("latest", NullValueHandling = NullValueHandling.Ignore)]
    public string LatestMessageThread { get; set; }
    
    [JsonProperty("oldest", NullValueHandling = NullValueHandling.Ignore)]
    public string OldestMessageThread { get; set; }
}