using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Domain.Slack.Channels;
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

    private readonly ILogger<IReportPullRequestsCommand> _logger;
    private readonly ISlackConversationClient _conversationClient;
    private readonly IGitlabLinkExtractor _linkExtractor;
    private readonly IGitlabProjectsClient _projectsClient;
    private readonly IGitlabProjectsCache _gitlabProjectsCache;
    private readonly ISlackMessagingClient _messagingClient;

    public ReportPullRequestsCommand(
        ILogger<IReportPullRequestsCommand> logger,
        IGitlabLinkExtractor linkExtractor,
        IGitlabProjectsCache projectsCache,
        IGitlabProjectsClient projectsClient,
        ISlackConversationClient conversationClient,
        ISlackMessagingClient messagingClient,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _logger = logger;
        _linkExtractor = linkExtractor;
        _gitlabProjectsCache = projectsCache;
        _projectsClient = projectsClient;
        _messagingClient = messagingClient;
        _conversationClient = conversationClient;
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
            await ReportBackMessage(request, $"Dont have access to {model.Channel}, maybe I should get invited there first");
            return;
        }

        var links = await GetGitlabLinks(model.Channel, period);
        foreach (var link in links)
        {
            var project = await _gitlabProjectsCache.GetProjectByNamespace(link.Name);
            if (project == null)
                continue;

            var approvals = await _projectsClient.GetApprovals(project.Id, link.Value);

            var isApproved = model.Approvals.HasValue
                ? approvals.ApprovedBy.Count >= model.Approvals.Value
                : approvals.ApprovalsLeft == 0;

            if (approvals.ApprovedBy.Count < 2)
            {
                int a = 0;
            }
            
            _logger.LogInformation($"Reading pull request for {project.PathWithNamespace}/{link.Value} = Approved by {approvals.ApprovedBy.Count} with status={approvals.State} and merge={approvals.MergeStatus}");
            
            if (!approvals.IsStillOpened 
                || approvals.Approved 
                || approvals.IsMerged 
                || isApproved)
                continue;

            await _messagingClient.PublishMessage(new()
            {
                ChannelId = request.Channel,
                Thread = request.MessageThread,
                Message = $"This was not being approved {project.PathWithNamespace}/{link.Value}"
            });
            _logger.LogInformation($"Project {project.Id} {project.PathWithNamespace}");
        }

        await ReportBackMessage(request,$"Message count is ${links.Count}");
    }

    
    private async Task<List<LinkExtractionResult>> GetGitlabLinks(string channel, DurationPeriod period)
    {
        var result = new HashSet<LinkExtractionResult>();

        var messageCount = 0;
        var currentDate = DateTime.UtcNow;
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
                if (msgDate == null ||  !period.IsDateInPeriod(currentDate, msgDate.Value))
                {
                    timeError = true;
                    break;
                }

                foreach (var link in _linkExtractor.ExtractLinks(msg.Text).ToList())
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
}