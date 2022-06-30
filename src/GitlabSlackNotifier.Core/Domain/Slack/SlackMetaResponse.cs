using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack;

public class SlackMetaResponse : SlackOkResponse
{
    [JsonProperty("response_metadata")] 
    public SlackMetaResponseMetadata Metadata { get; set; } = new ();
}

public class SlackMetaResponseMetadata
{
    [JsonProperty("next_cursor")]
    public string NextCursor { get; set; }
}