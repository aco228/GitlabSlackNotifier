using GitlabSlackNotifier.Core.Domain;
using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.Persistency;
using GitlabSlackNotifier.Core.Domain.Slack;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;
using GitlabSlackNotifier.Core.Domain.Slack.Users;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Persistency;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Utilities.Slack;
using GitlabSlackNotifier.Core.Services.ZenQuote;

namespace GitlabSlackNotifier.Core.Infrastructures.Utilities.Slack;

public class ConstructReportMessageUtility : IConstructReportMessageUtility
{
    private bool _stateIsSet = false;
    private readonly ISlackUserCache _slackUserCache;
    private readonly IJiraConfigurationSection _jiraConfigurationSection;
    private readonly ISlackConfigurationSection _slackConfigurationSection;
    private readonly ISlackMessagingClient _messagingClient;
    private readonly IZenQuoteRandomClient _zenQuoteRandomClient;
    private readonly IGitlabMergeRequestClient _mergeRequestClient;
    private readonly IUserRepository _userRepository;
    
    private List<BlockBase> Blocks { get; set; } = new();
    private string ChannelId { get; set; }
    private string OutputChannelId { get; set; }
    private int ApprovalsRequired { get; set; } = -1;

    public ConstructReportMessageUtility (
        ISlackUserCache slackUserCache, 
        IJiraConfigurationSection jiraConfigurationSection, 
        ISlackConfigurationSection slackConfigurationSection, 
        ISlackMessagingClient messagingClient, 
        IZenQuoteRandomClient zenQuoteRandomClient, 
        IGitlabMergeRequestClient mergeRequestClient, 
        IUserRepository userRepository)
    {
        _slackUserCache = slackUserCache;
        _jiraConfigurationSection = jiraConfigurationSection;
        _slackConfigurationSection = slackConfigurationSection;
        _messagingClient = messagingClient;
        _zenQuoteRandomClient = zenQuoteRandomClient;
        _mergeRequestClient = mergeRequestClient;
        _userRepository = userRepository;
    }
    
    public void SetState(string channelId, string outputChannelId, int approvalRequired)
    {
        ChannelId = channelId;
        OutputChannelId = outputChannelId;
        ApprovalsRequired = approvalRequired;
        _stateIsSet = true;
    }

    public async Task SendPullRequestMessageThread(ReportMergeRequest request)
    {
        if (!_stateIsSet)
            throw new ArgumentException("State is not set");
        
        var slackUser = await _slackUserCache.GetUser(request.Link.Author);
        if (slackUser?.Ok == false)
            return;

        
        
        var jiraTicketNotifier = string.Empty;
        if (request.JiraIssue != null)
            jiraTicketNotifier =
                $"{SlackEmoji.Jira} {$"Jira {request.JiraIssue.Key}".ToSlackLink(_jiraConfigurationSection.ConstructUrl(request.JiraIssue.Key))} " 
                + Environment.NewLine;
                
        // add title
        Blocks.Add(new TextSection(
            request.JiraIssue != null 
                ? $"*{request.JiraIssue.Key} : {request.JiraIssue.Title}*" 
                : $"*{request.Approvals.Title}*"));
        
        // add approved by section if exists
        if(request.ApprovedBy.Count > 0)
            AddContextApprovals(request.Approvals);

        // get link to the original message thread
        var archiveLink = SlackModelsExtensions.GetArchiveLink(
            _slackConfigurationSection.GetConfiguration()!.SlackOwner,
            ChannelId, 
            request.Link.OriginalThreadId);

        // get missing approvals count
        var missingApprovals = ApprovalsRequired - request.Approvals.ApprovedBy.Count;
        if (missingApprovals < 0) missingApprovals = 0;
        
        var codeOwnersMessage = string.Empty;
        
        // mention code owners if they did not approved this PR
        if (request.NotApprovedByCodeOwners)
        {
            codeOwnersMessage = Environment.NewLine + $"{SlackEmoji.TopHat} Missing code owners: ";
            foreach (var codeOwner in request.CodeOwners.Where(x => request.ApprovedBy[x.Gitlab] == null))
            {
                codeOwnersMessage += codeOwner.Slack.ToSlackUserMention() + " ";
                // TODO send private message to slack (maybe?)
            }
        }

        // main informations
        var messageBody =
            $"Missing *{missingApprovals}* approvals! Submitted by: *{slackUser!.Profile.display_name}* {request.Link.DaysDifference} days ago" +
            Environment.NewLine
            + $"{SlackEmoji.Thread} {"Original thread".ToSlackLink(archiveLink)} " + Environment.NewLine
            + jiraTicketNotifier
            + $"{SlackEmoji.PointRight} Pull request {request.Link.ProjectName}/{request.Link.PullRequestId} ".ToSlackLink(request.Link.RawValue)
            + codeOwnersMessage;
        
        AddAuthorInformations(messageBody, slackUser);
        Blocks.Add(new Divider());

        var message = await _messagingClient.PublishMessage(new()
        {
            Blocks = Blocks,
            ChannelId = OutputChannelId,
            UnfurLinks = false
        });

        if (request.JiraIssue == null)
            await _messagingClient.PublishMessage(new()
            {
                Thread = message.ts,
                ChannelId = message.channel,
                Message = $"{request.Link.Author.ToSlackUserMention()} title of the PR does not contain jira ticket reference. Can you please check and fix that?"
            });

        await ReportUnresolvedComments(request, message);
        await ReportIfPullRequestHasConflicts(request, message);
        
        Blocks = new();
    }

    public async Task OnTheEnd(int messagesRead, int linksCount, int dayDifference, int approvals)
    {
        var blocks = new List<BlockBase>();
        var quote = await _zenQuoteRandomClient.GetRandomQuote();

        blocks.Add(new TextSection($"{GlobalConstants.Slack.HereMention} hello there! " +
                                   $"Today I have read *{messagesRead}* messages and checked *{linksCount}* gitlab links for past *{dayDifference}* days, with criteria that MR has at least *{approvals}* approvals, " +
                                   $"which means that you can at least resolve couple of those MR's, as I would not like going through same links each morning. Thanks!"));
        
        blocks.Add(new Divider());
        
        if(quote != null)
            blocks.Add(new TextSection($"Here is a random quote I have prepared for you by {quote.Author}, to motivate you a bit:" 
                                       + Environment.NewLine + Environment.NewLine 
                                       + $"> {quote.Text}"));

        await _messagingClient.PublishMessage(new()
        {
            Blocks = blocks,
            ChannelId = OutputChannelId,
            UnfurLinks = false,
        });
    }

    private void AddContextApprovals(
        GitlabApprovalsResponse approvals)
    {
        var contextSection = new ContextSection();
        contextSection.Elements.Add(new TextElement("Approved by: "));
        
        foreach (var approval in approvals.ApprovedBy)
            contextSection.Elements.Add(new ImageElement(approval.User.PictureUrl, approval.User.Username));
        
        Blocks.Add(contextSection);
    }

    private void AddAuthorInformations(string text, SlackUserResponse user)
    {
        Blocks.Add(new ImageSection(text, user.Profile.image_192, user.Profile.display_name));
    }

    private async Task ReportUnresolvedComments(ReportMergeRequest request, PublishMessageResponse message)
    {
        var unresolvedUsersMentions = "";
        var hasUnresolvedComments = false;
        var usersCache = new List<string> { request.Link.Author };
        
        foreach (var comment in await _mergeRequestClient.GetMergeRequestNotes(request.Approvals.ProjectId, request.Approvals.MergeRequestId))
        {
            if (!comment.IsUserComment || comment.Resolved) continue;
            
            var user = _userRepository.GetUserIdentifier(comment.Author.Username);
            if (user == null || usersCache.Contains(user.Slack))
                continue;

            usersCache.Add(user.Slack);
            hasUnresolvedComments = true;
            unresolvedUsersMentions += $" {user.Slack.ToSlackUserMention()}";
        }

        if (hasUnresolvedComments)
            await _messagingClient.PublishMessage(new ()
            {
                Thread = message.ts,
                ChannelId = message.channel,
                Message = $"{request.Link.Author.ToSlackUserMention()} you have some unresolved comments from {unresolvedUsersMentions}",
            });
    }

    private async Task ReportIfPullRequestHasConflicts(ReportMergeRequest request, PublishMessageResponse message)
    {
        if (!request.Approvals.HasConflicts)
            return;

        await _messagingClient.PublishMessage(new ()
        {
            Thread = message.ts,
            ChannelId = message.channel,
            Message = $"{request.Link.Author.ToSlackUserMention()} this pull request cannot be merged. Probably has conflicts. Please check",
        });
    }
    
    
}