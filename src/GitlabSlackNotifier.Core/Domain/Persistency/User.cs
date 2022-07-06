using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Persistency;

public class User
{
    [JsonProperty("slack")]
    public string Slack { get; set; }
    
    [JsonProperty("gitlab")]
    public string Gitlab { get; set; }
}