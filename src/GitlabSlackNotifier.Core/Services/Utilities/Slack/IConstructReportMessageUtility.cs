using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Services.Persistency;

namespace GitlabSlackNotifier.Core.Services.Utilities.Slack;

public interface IConstructReportMessageUtility
{
    void SetState(string channelId, string outputChannelId, int approvalRequired);

    Task OnTheEnd(int messagesRead, int linksCount, int dayDifference, int approvals);

    Task SendPullRequestMessageThread(
        LinkExtractionResult link,
        IUserCollection approvedBy,
        IUserCollection codeOwners,
        GitlabApprovalsResponse approvals,
        bool notApprovedByCodeOwners);
}