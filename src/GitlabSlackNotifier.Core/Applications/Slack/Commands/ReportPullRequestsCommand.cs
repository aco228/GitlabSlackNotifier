using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using GitlabSlackNotifier.Core.Services.Utilities.Gitlab;
using GitlabSlackNotifier.Core.Services.Utilities.Slack;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Applications.Slack.Commands;

public interface IReportPullRequestsCommand : ISlackApplicationCommand { }

public class ReportPullRequestsCommand :
    SlackCommandComposeBase<ReportPullRequestCommandModel>,
    IReportPullRequestsCommand
{
    public override string CommandName =>  "report";
    public override SlackCommandType CommandType =>  SlackCommandType.Mention;
    protected override string Description { get; } = "Report/remind about gitlab links in channel that are still not approved";

    private readonly ILogger<IReportPullRequestsCommand> _logger;
    private readonly ISlackConversationClient _conversationClient;
    private readonly IGitlabProjectsCache _gitlabProjectsCache;
    private readonly IGetSlackMessageLinkUtility _getSlackMessageLinkUtility;
    private readonly IConstructReportMessageUtility _constructReportMessageUtility;
    private readonly IGetApprovalRulesUtility _getApprovalRulesUtility;

    public ReportPullRequestsCommand(
        ILogger<IReportPullRequestsCommand> logger,
        IGitlabProjectsCache projectsCache,
        ISlackConversationClient conversationClient,
        IServiceProvider serviceProvider,
        IGetSlackMessageLinkUtility getSlackMessageLinkUtility, 
        IConstructReportMessageUtility constructReportMessageUtility, 
        IGetApprovalRulesUtility getApprovalRulesUtility)
        : base(serviceProvider)
    {
        _logger = logger;
        _gitlabProjectsCache = projectsCache;
        _conversationClient = conversationClient;
        _getSlackMessageLinkUtility = getSlackMessageLinkUtility;
        _constructReportMessageUtility = constructReportMessageUtility;
        _getApprovalRulesUtility = getApprovalRulesUtility;
    }

    protected override async Task Process(
        SlackCommandRequest request, 
        ReportPullRequestCommandModel model)
    {
        var channel = await _conversationClient
            .GetConversation(new() { Channel = model.Channel});

        if (!channel.Ok)
        {
            await ReportBackWithLog(request,$"Dont have access to {model.Channel}, maybe I should get invited there first");
            return;
        }

        var outputChannel = model.Output ?? model.Channel;
        _constructReportMessageUtility.SetState(model.Channel, outputChannel, model.Approvals);
        
        var linksResponse = await _getSlackMessageLinkUtility.GetLinksFromSlackChannel(model.Channel, model.DurationPeriod, model.SkipPeriod);

        int foundNonApprovedMRs = 0;
        ReportBackWithLog(request, $"Starting to process {linksResponse.LinkCount} gitlab links").ConfigureAwait(false);
        
        for (var i = linksResponse.Links.Count - 1; i >= 0; i--)
        {
            var link = linksResponse.Links[i];
            
            var project = await _gitlabProjectsCache.GetProjectByNamespace(link.ProjectName);
            if (project == null)
                continue;

            var (approvals, usersApproved) = await _getApprovalRulesUtility.GetPullRequestApprovals(project.Id, link.PullRequestId);
            var (approvalRules, usersOwners) = await _getApprovalRulesUtility.GetApprovalRulesUsers(project.Id, link.PullRequestId);

            _logger.LogInformation($"Reading pull request for {project.PathWithNamespace}/{link.PullRequestId} = Approved by {approvals.ApprovedBy.Count} with status={approvals.State} and merge={approvals.MergeStatus}");

            var notApprovedByCodeOwner = !usersApproved.Any(x => usersOwners.Contains(x));
            
            if (!approvals.IsStillOpened
                || approvals.Approved
                || (!notApprovedByCodeOwner && approvals.ApprovedBy.Count >= model.Approvals))
                continue;

            ++foundNonApprovedMRs;

            await _constructReportMessageUtility.SendPullRequestMessageThread(
                link, 
                usersApproved, 
                usersOwners, 
                approvals,
                notApprovedByCodeOwner);
            
            _logger.LogInformation($"Project {project.Id} {project.PathWithNamespace}");
        }

        if (foundNonApprovedMRs == 0)
        {
            // TODO: Make some report
            await ReportBackMessage(request, $"Could not find any interesting PR to report");
            return;
        }
        
        await _constructReportMessageUtility.OnTheEnd(
            linksResponse.MessagesRead, 
            linksResponse.LinkCount, 
            model.DurationPeriod.GetDayDifference(), 
            model.Approvals);
    }
}
