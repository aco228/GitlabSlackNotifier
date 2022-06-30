using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class ConversationListResponse : SlackMetaResponse
{
    [JsonProperty("channels")]
    public List<SlackChannelResponse> Channels { get; set; }
}