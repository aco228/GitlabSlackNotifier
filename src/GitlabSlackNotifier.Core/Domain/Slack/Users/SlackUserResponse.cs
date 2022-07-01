using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Users;

public class SlackUserResponse : SlackOkResponse
{
    [JsonProperty("profile")]
    public SlackUser Profile { get; set; }
}