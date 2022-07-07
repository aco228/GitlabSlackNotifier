using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public class GitlabApprovalsResponse
{
    [JsonProperty("iid")]
    public int MergeRequestId { get; set; }
    
    [JsonProperty("project_id")]
    public long ProjectId { get; set; }
    
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

    [JsonIgnore]
    public bool HasConflicts => new[] {"cannot_be_merged", "cannot_be_merged_recheck"}.Contains(MergeStatus);

}

public class GitlabUserApprovalsResponse
{
    [JsonProperty("user")]
    public GitlabUserResponse User { get; set; }
}