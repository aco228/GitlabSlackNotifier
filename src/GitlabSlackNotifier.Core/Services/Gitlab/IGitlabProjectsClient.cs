using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

namespace GitlabSlackNotifier.Core.Services.Gitlab;

public interface IGitlabProjectsClient
{
    Task<List<GitlabProjectResponse>> GetProjects(SearchProjectsRequest request);
    Task<GitlabApprovalsResponse> GetApprovals(long projectId, int mergeRequestId);
    Task<List<GitlabApprovalRulesResponse>?> GetApprovalRules(long projectId, int mergeRequestId);
}