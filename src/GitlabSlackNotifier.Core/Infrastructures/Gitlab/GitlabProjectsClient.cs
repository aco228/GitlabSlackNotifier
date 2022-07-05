using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Services.Gitlab;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabProjectsClient : IGitlabProjectsClient
{
    private readonly IGitlabHttpClient _client;
    
    public GitlabProjectsClient (IGitlabHttpClient client)
    {
        _client = client;
    }

    public Task<List<GitlabProjectResponse>> GetProjects(SearchProjectsRequest request)
        => _client.Get<List<GitlabProjectResponse>>("projects", request);

    public Task<GitlabApprovalsResponse> GetApprovals(long projectId, int mergeRequestId)
        => _client.Get<GitlabApprovalsResponse>($"projects/{projectId}/merge_requests/{mergeRequestId}/approvals");

    public Task<List<GitlabApprovalRulesResponse>?> GetApprovalRules(long projectId, int mergeRequestId)
        => _client.Get<List<GitlabApprovalRulesResponse>?>($"projects/{projectId}/merge_requests/{mergeRequestId}/approval_rules");
}