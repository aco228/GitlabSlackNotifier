using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Services.Persistency;

namespace GitlabSlackNotifier.Core.Services.Utilities.Slack;

public interface IConstructReportMessageUtility
{
    List<BlockBase> Blocks { get; }
    string ChannelId { get; set; }
    int ApprovalsRequired { get; set; }

    void SetState(string channelId, int approvalRequired);

    void OnTheEnd(int messagesRead, int linksCount, int dayDifference, int approvals);

    Task AddPullRequestSection(
        LinkExtractionResult link,
        IUserCollection approvedBy,
        IUserCollection codeOwners,
        GitlabApprovalsResponse approvals,
        bool notApprovedByCodeOwners);
}