using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Infrastructures.Gitlab;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Applications.Slack.Commands;

public interface IReportPullRequestsCommand : ISlackApplicationCommand { }

public class ReportPullRequestsCommand :
    SlackCommandComposeBase<ReportPullRequestCommandModel>,
    IReportPullRequestsCommand
{
    public override string CommandName =>  "report";
    public override SlackCommandType CommandType =>  SlackCommandType.Mention;
    public override string Description { get; } = "Report/remind about gitlab links in channel that are still not approved";

    private readonly ILogger<IReportPullRequestsCommand> _logger;
    private readonly ISlackConversationClient _conversationClient;
    private readonly IGitlabSlackLinkExtractor _slackLinkExtractor;
    private readonly IGitlabProjectsClient _projectsClient;
    private readonly IGitlabProjectsCache _gitlabProjectsCache;
    private readonly ISlackMessagingClient _messagingClient;
    private readonly ISlackConfigurationSection _slackConfiguration;
    private readonly ISlackUserCache _slackUserCache;

    public ReportPullRequestsCommand(
        ILogger<IReportPullRequestsCommand> logger,
        IGitlabSlackLinkExtractor slackLinkExtractor,
        IGitlabProjectsCache projectsCache,
        IGitlabProjectsClient projectsClient,
        ISlackConversationClient conversationClient,
        ISlackMessagingClient messagingClient,
        IServiceProvider serviceProvider,
        ISlackUserCache slackUserCache,
        ISlackConfigurationSection slackConfigurationSection)
        : base(serviceProvider)
    {
        _logger = logger;
        _slackLinkExtractor = slackLinkExtractor;
        _gitlabProjectsCache = projectsCache;
        _projectsClient = projectsClient;
        _messagingClient = messagingClient;
        _conversationClient = conversationClient;
        _slackConfiguration = slackConfigurationSection;
        _slackUserCache = slackUserCache;
    }

    protected override async Task Process(
        SlackCommandRequest request, 
        ReportPullRequestCommandModel model)
    {
        if (!model.IsModelValid())
        {
            await ReportBackWithLog(request, $"Could not parse duration from string ${model.Duration}");
            return;
        }

        var channel = await _conversationClient
            .GetConversation(new() { Channel = model.Channel});

        if (!channel.Ok)
        {
            await ReportBackWithLog(request,$"Dont have access to {model.Channel}, maybe I should get invited there first");
            return;
        }

        var slackBlocks = new ReportPullRequestCommandSlackMessage(model);


        var links = await GetGitlabLinks(model);
        int foundNonApprovedMRs = 0;

        foreach (var link in links)
        {
            var project = await _gitlabProjectsCache.GetProjectByNamespace(link.ProjectName);
            if (project == null)
                continue;

            var approvals = await _projectsClient.GetApprovals(project.Id, link.PullRequestId);

            _logger.LogInformation(
                $"Reading pull request for {project.PathWithNamespace}/{link.PullRequestId} = Approved by {approvals.ApprovedBy.Count} with status={approvals.State} and merge={approvals.MergeStatus}");

            if (!approvals.IsStillOpened
                || approvals.Approved
                || approvals.ApprovedBy.Count >= model.Approvals)
                continue;

            ++foundNonApprovedMRs;
            await AddSlackReportSectionsForPullRequest(slackBlocks, link, approvals);
            
            _logger.LogInformation($"Project {project.Id} {project.PathWithNamespace}");
        }

        if (foundNonApprovedMRs == 0)
        {
            // TODO: Make some report
            await ReportBackMessage(request, $"Could not find any interesting PR to repoert");
            return;
        }

        await _messagingClient.PublishMessage(new ()
        {
            Blocks = slackBlocks.Blocks,
            ChannelId = "C03MLTPSGH3", // TODO: take correct channel
            UnfurLinks = false,
        });
    }


    private async Task<List<LinkExtractionResult>> GetGitlabLinks(ReportPullRequestCommandModel model)
    {
        var result = new HashSet<LinkExtractionResult>();

        var messageCount = 0;
        var conversationRequest = new ConversationMessagesRequest { Channel = model.Channel };

        for (;;)
        {
            var messages = await _conversationClient.GetMessages(conversationRequest);

            if (!messages.Ok
                || messages.Messages.Count == 0)
                break;

            var timeError = false;

            foreach (var msg in messages.Messages)
            {
                ++messageCount;
                var msgDate = msg.GetDate();
                if (msgDate == null)
                {
                    timeError = true;
                    break;
                }
                
                if (model.SkipPeriod.IsDateInPeriod(msgDate.Value))
                    continue;
                
                if (!model.DurationPeriod.IsDateInPeriod(msgDate.Value))
                {
                    timeError = true;
                    break;
                }

                foreach (var link in _slackLinkExtractor.ExtractLinks(msg).ToList())
                    result.Add(link);
            }

            if (timeError || !messages.HasMore)
                break;

            conversationRequest.LatestMessageThread = messages.Messages.First().MessageThread;
            conversationRequest.OldestMessageThread = messages.Messages.Last().MessageThread;
        }

        _logger.LogInformation($"Read {messageCount} for channel={model.Channel}");
        return result.ToList();
    }

    private async Task AddSlackReportSectionsForPullRequest(
        ReportPullRequestCommandSlackMessage slackMessage,
        LinkExtractionResult link,
        GitlabApprovalsResponse approvals)
    {
        var slackUser = await _slackUserCache.GetUser(link.Author);
        if (slackUser?.Ok == false)
            return;
        
        slackMessage.AddTitle($"*{approvals.Title}*");
        slackMessage.AddContextApprovals(approvals);
        
        var archiveLink =
            $"https://{_slackConfiguration.GetConfiguration()!.SlackOwner}/archives/{slackMessage.Model.Channel}/p{link.OriginalThreadId.Replace(".", "")}";

        var missingApprovals = slackMessage.Model.Approvals - approvals.ApprovedBy.Count;
        var prInformations = $"Missing *{missingApprovals}* approvals! Submited by: *{slackUser.Profile.display_name}* {link.DaysDifference} days ago";
        
        var messageBody =
            prInformations 
            + Environment.NewLine
            + $"{"Original thread".ToSlackLink(archiveLink)} "
            + Environment.NewLine
            + ":point_right:  "
            + $" Pull request {link.ProjectName}/{link.PullRequestId} ".ToSlackLink(link.RawValue)
            + "  :point_left:";

        slackMessage.AddAuthorInformations(messageBody, slackUser);
        slackMessage.AddDivider();
    }

    private Task ReportBackWithLog(SlackCommandRequest request, string message)
    {
        _logger.LogInformation(message);
        return ReportBackMessage(request, message);
    }
}