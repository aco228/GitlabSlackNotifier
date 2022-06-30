using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class ConversationMessagesResponse : SlackHasMoreResponse
{
    [JsonProperty("messages")]
    public List<SlackMessageResponse> Messages { get; set; }
}