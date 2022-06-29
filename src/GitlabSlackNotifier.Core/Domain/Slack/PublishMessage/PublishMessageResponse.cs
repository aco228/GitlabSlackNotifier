using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack;

public class PublishMessageResponse
{
    [JsonProperty("ok")]
    public bool ok { get; set; }
    
    [JsonProperty("channel")]
    public string channel { get; set; }
    
    [JsonProperty("ts")]
    public string ts { get; set; }   
}