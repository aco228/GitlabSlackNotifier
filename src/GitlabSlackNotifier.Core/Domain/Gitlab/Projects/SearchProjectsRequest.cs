using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public record SearchProjectsRequest
{
    [JsonProperty("archived")]
    public bool Archived { get; set; } = false;
    
    [JsonProperty("membership")]
    public bool Membership { get; set; } = true;
    
    [JsonProperty("search")]
    public string Search { get; set; }
    
    [JsonProperty("search_namespaces")]
    public bool SearchNamespaces { get; set; } = true;

}