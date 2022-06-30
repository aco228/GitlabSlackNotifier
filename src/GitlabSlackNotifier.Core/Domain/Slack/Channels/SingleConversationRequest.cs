using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class SingleConversationRequest
{
    [JsonProperty("channel")]
    public string Channel { get; set; }
}