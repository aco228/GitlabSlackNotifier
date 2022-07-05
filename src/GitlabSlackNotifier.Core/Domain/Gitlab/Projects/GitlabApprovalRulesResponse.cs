using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

public class GitlabApprovalRulesResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("rule_type")]
    public string RuleType { get; set; }
    
    [JsonProperty("eligible_approvers")]
    public List<GitlabUserResponse> EligibleApprovers { get; set; }
}