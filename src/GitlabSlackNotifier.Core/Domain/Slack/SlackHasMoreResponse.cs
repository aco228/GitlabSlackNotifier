using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack;

public class SlackHasMoreResponse : SlackOkResponse
{
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }
}