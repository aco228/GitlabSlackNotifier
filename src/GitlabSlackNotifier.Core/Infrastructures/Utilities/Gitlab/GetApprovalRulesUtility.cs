using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.Persistency;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Persistency;
using GitlabSlackNotifier.Core.Services.Utilities.Gitlab;

namespace GitlabSlackNotifier.Core.Infrastructures.Utilities.Gitlab;

public class GetApprovalRulesUtility : IGetApprovalRulesUtility
{
    private readonly IUserRepository _userRepository;
    private readonly IGitlabProjectsClient _projectsClient;
    
    public GetApprovalRulesUtility ( 
        IGitlabProjectsClient projectsClient, 
        IUserRepository userRepository)
    {
        _projectsClient = projectsClient;
        _userRepository = userRepository;
    }

    public async Task<Tuple<GitlabApprovalsResponse, IUserCollection>> GetPullRequestApprovals(long projectId, int pullRequestId)
    {
        var response = new UserCollection();
        var approvals = await _projectsClient.GetApprovals(projectId, pullRequestId);

        foreach (var gitlabUser in approvals.ApprovedBy)
        {
            var user = _userRepository.GetUserIdentifier(gitlabUser.User.Username);
            if(user != null)
                response.Add(user);
        }

        return new(approvals, response);
    }

    public async Task<Tuple<List<GitlabApprovalRulesResponse>, IUserCollection>> GetApprovalRulesUsers(long projectId, int pullRequestId)
    {
        var response = new UserCollection();
        var approvals = await _projectsClient.GetApprovalRules(projectId, pullRequestId);

        foreach (var rule in approvals)
            foreach (var gitlabUser in rule.EligibleApprovers)
            {
                var user = _userRepository.GetUserIdentifier(gitlabUser.Username);
                if(user != null)
                    response.Add(user);
            }

        return new (approvals, response);
    }
}