using System.Text;
using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;
using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Infrastructures.Gitlab;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Applications.Slack.Commands;

public interface IReportPullRequestsCommand : ISlackApplicationCommand
{
}

public class ReportPullRequestsCommand :
    SlackCommandComposeBase<ReportPullRequestCommandModel>,
    IReportPullRequestsCommand
{
    public override string CommandName =>  "report";
    public override SlackCommandType CommandType =>  SlackCommandType.Mention;

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

    protected override async Task Process(SlackCommandRequest request, ReportPullRequestCommandModel model)
    {
        if (!model.GetDuration(out var period))
        {
            _logger.LogInformation($"Could not parse duration from string ${model.Duration}");
            await ReportBackMessage(request, $"Could not parse duration from string ${model.Duration}");
            return;
        }

        var channel = await _conversationClient
            .GetConversation(new() { Channel = model.Channel});

        if (!channel.Ok)
        {
            _logger.LogInformation($"Could not find channel {model.Channel}");
            await ReportBackMessage(request,
                $"Dont have access to {model.Channel}, maybe I should get invited there first");
            return;
        }

        var slackBlocks = new List<BlockBase>();
        slackBlocks.Add(new HeaderElement("MR's without enough approvals"));
        slackBlocks.Add(new TextSection($"Gone {period.GetDayDifference()} days in past."));
        slackBlocks.Add(new TextSection(model.Approvals.HasValue
            ? $"Criteria is that MR has at least {model.Approvals.Value} approvals"
            : "Criteria is that MR has approvals set by project"));
        slackBlocks.Add(new Divider());


        var links = await GetGitlabLinks(model.Channel, period);
        int foundNonApprovedMRs = 0;

        foreach (var link in links)
        {
            var project = await _gitlabProjectsCache.GetProjectByNamespace(link.ProjectName);
            if (project == null)
                continue;

            var approvals = await _projectsClient.GetApprovals(project.Id, link.PullRequestId);

            var isApproved = model.Approvals.HasValue
                ? approvals.ApprovedBy.Count >= model.Approvals.Value
                : approvals.ApprovalsLeft == 0;

            _logger.LogInformation(
                $"Reading pull request for {project.PathWithNamespace}/{link.PullRequestId} = Approved by {approvals.ApprovedBy.Count} with status={approvals.State} and merge={approvals.MergeStatus}");

            if (!approvals.IsStillOpened
                || approvals.Approved
                || isApproved)
                continue;

            ++foundNonApprovedMRs;
            slackBlocks.AddRange(await GetPullRequestBlock(model.Channel, link, approvals,
                model.Approvals ?? approvals.ApprovalsRequired));
            
            _logger.LogInformation($"Project {project.Id} {project.PathWithNamespace}");
        }

        if (foundNonApprovedMRs == 0)
        {
            // TODO: Make some report
            return;
        }

        await _messagingClient.PublishMessage(new ()
        {
            Blocks = slackBlocks,
            ChannelId = "C03MLTPSGH3", // TODO: take correct channel
            UnfurLinks = false,
        });
    }


    private async Task<List<LinkExtractionResult>> GetGitlabLinks(string channel, DurationPeriod period)
    {
        var result = new HashSet<LinkExtractionResult>();

        var messageCount = 0;
        var conversationRequest = new ConversationMessagesRequest { Channel = channel };

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
                if (msgDate == null ||  !period.IsDateInPeriod(msgDate.Value))
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

        _logger.LogInformation($"Read {messageCount} for channel={channel}");
        return result.ToList();
    }

    private async Task<List<BlockBase>> GetPullRequestBlock(
        string channelId,
        LinkExtractionResult link,
        GitlabApprovalsResponse approvals,
        int expectedApprovals)
    {
        var result = new List<BlockBase>();
        var days = (int) (DateTime.UtcNow - link.Created).TotalDays;
        var slackUser = await _slackUserCache.GetUser(link.Author);
        
        result.Add(new TextSection($"*{approvals.Title}*"));

        var contextSection = new ContextSection();
        foreach (var approval in approvals.ApprovedBy)
            contextSection.Elements.Add(new ImageElement(approval.User.PictureUrl, approval.User.Username));
        
        contextSection.Elements.Add(new TextElement($" (Got {approvals.ApprovedBy.Count}/{expectedApprovals}) {days} days old"));
        result.Add(contextSection);

        var archiveLink =
            $"https://{_slackConfiguration.GetConfiguration()!.SlackOwner}/archives/{channelId}/p{link.OriginalThreadId.Replace(".", "")}";

        var messageBody =
            $"{slackUser.Profile.display_name} asked in this {"thread".ToSlackLink(archiveLink)} for approval of *{approvals.Title}* and only got {approvals.ApprovedBy.Count} approves."
            + Environment.NewLine
            + $"Find MR at this link " + link.RawValue;

        
        if (slackUser != null)
            result.Add(new ImageSection(messageBody, slackUser.Profile.image_192, slackUser.Profile.display_name));
        else
            result.Add(new TextSection(messageBody));
        
        result.Add(new Divider());
        return result;
    }
}