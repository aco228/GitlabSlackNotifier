using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public class GitlabUserResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("username")]
    public string username { get; set; }
    
    [JsonProperty("name")]
    public string name { get; set; }
}