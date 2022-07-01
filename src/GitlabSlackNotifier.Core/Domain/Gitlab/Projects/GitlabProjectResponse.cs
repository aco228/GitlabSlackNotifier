using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public class GitlabProjectResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("path")]
    public string Path { get; set; }
    
    [JsonProperty("path_with_namespace")]
    public string PathWithNamespace { get; set; }
}