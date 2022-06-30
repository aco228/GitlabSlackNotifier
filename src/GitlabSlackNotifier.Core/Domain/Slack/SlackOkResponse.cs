using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack;

public class SlackOkResponse
{
    [JsonProperty("ok")]
    public bool Ok { get; set; }
}