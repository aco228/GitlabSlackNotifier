using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;

namespace GitlabSlackNotifier.Core.Services.Gitlab;

public interface IGitlabProjectsCache
{
    Task<GitlabProjectResponse?> GetProjectByNamespace(string namespacePath);
    Task<List<GitlabApprovalRulesResponse>> GetRulesForMergeRequest(long projectId, int mergeRequestId);
}