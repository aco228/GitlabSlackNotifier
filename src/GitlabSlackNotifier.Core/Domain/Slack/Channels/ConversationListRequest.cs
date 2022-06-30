using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class ConversationListRequest
{
    [JsonIgnore]
    public bool ExcludeArchived { get; set; } = true;
    
    [JsonIgnore]
    public List<SlackChannelType> Types { get; set; } = new() { SlackChannelType.public_channel, SlackChannelType.private_channel };
    
    [JsonProperty("limit")]
    public int Limit { get; set; } = 100;
    
    [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
    public string Cursor { get; set; }

    [JsonProperty("exclude_archived")] 
    public string  Arg_ExcludeArchived => ExcludeArchived ? "true" : "false";

    [JsonProperty("types")]
    public string Arg_Types
    {
        get
        {
            var result = "";
            foreach (var type in Types)
                result += (!string.IsNullOrEmpty(result) ? "," : "") + type.ToString();
            return result;
        }
    }
}