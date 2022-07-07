using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.MergeRequests;

public class GitlabMergeRequestNote
{
    [JsonProperty("type")]
    public string? Type { get; set; }
    
    [JsonProperty("author")]
    public GitlabUserResponse Author { get; set; }
    
    [JsonProperty("resolved")]
    public bool Resolved { get; set; }
    
    [JsonProperty("system")]
    public bool SystemNote { get; set; }

    [JsonIgnore] public bool IsUserComment => Type?.Equals("DiffNote") == true && !SystemNote;

}