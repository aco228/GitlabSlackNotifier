using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public enum SlackChannelType
{
    public_channel,
    private_channel,
    mpim,
    im,
}

public class SlackChannelResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("name_normalized")]
    public string NameNormalized { get; set; }
    
    [JsonProperty("is_channel")]
    public bool IsChannel { get; set; }
    
    [JsonProperty("is_group")]
    public bool IsGroup { get; set; }
    
    [JsonProperty("is_archived")]
    public bool IsArchived { get; set; }
    
    [JsonProperty("is_private")]
    public bool IsPrivate { get; set; }
    
    [JsonProperty("is_member")]
    public bool IsMember { get; set; }
}