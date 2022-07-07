using GitlabSlackNotifier.Core.Domain.Application.Commands;

namespace GitlabSlackNotifier.Core.Services.Utilities.Slack;

public interface IConstructReportMessageUtility
{
    void SetState(string channelId, string outputChannelId, int approvalRequired);

    Task OnTheEnd(int messagesRead, int linksCount, int dayDifference, int approvals);

    Task SendPullRequestMessageThread(ReportMergeRequest request);
}