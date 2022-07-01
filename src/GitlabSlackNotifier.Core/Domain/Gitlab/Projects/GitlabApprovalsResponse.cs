using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public class GitlabApprovalsResponse
{
    [JsonProperty("iid")]
    public int MergeRequestId { get; set; }
    
    [JsonProperty("project_id")]
    public string ProjectId { get; set; }
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("state")]
    public string State { get; set; }
    
    [JsonProperty("merge_status")]
    public string MergeStatus { get; set; }
    
    [JsonProperty("approved")]
    public bool Approved { get; set; }
    
    [JsonProperty("approvals_required")]
    public int ApprovalsRequired { get; set; }
    
    [JsonProperty("approvals_left")]
    public int ApprovalsLeft { get; set; }
    
    [JsonProperty("approved_by")]
    public List<GitlabUserApprovalsResponse> ApprovedBy { get; set; }

    [JsonIgnore]
    public bool IsStillOpened => State.Equals("opened");

}

public class GitlabUserApprovalsResponse
{
    [JsonProperty("user")]
    public GitlabUserResponse User { get; set; }
}