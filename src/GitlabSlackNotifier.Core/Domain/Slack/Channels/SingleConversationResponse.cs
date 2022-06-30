using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class SingleConversationResponse : SlackOkResponse
{
    [JsonProperty("channel")]
    public SlackChannelResponse Channel { get; set; }
}