using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Services.Persistency;

namespace GitlabSlackNotifier.Core.Services.Utilities.Gitlab;

public interface IGetApprovalRulesUtility
{
    Task<Tuple<GitlabApprovalsResponse, IUserCollection>> GetPullRequestApprovals(long projectId, int pullRequestId);

    Task<Tuple<List<GitlabApprovalRulesResponse>, IUserCollection>> GetApprovalRulesUsers(long projectId, int pullRequestId);
}