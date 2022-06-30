using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.AboutMeModels;

public class GitlabAboutMeResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
}