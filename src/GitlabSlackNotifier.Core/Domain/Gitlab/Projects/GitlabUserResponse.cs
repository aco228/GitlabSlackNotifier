using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public class GitlabUserResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("avatar_url")]
    public string PictureUrl { get; set; }
}