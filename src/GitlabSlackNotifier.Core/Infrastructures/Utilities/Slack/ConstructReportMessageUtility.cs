using GitlabSlackNotifier.Core.Domain;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.Slack;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;
using GitlabSlackNotifier.Core.Domain.Slack.Users;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
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
    
    private List<BlockBase> Blocks { get; set; } = new();
    private string ChannelId { get; set; }
    private string OutputChannelId { get; set; }
    private int ApprovalsRequired { get; set; } = -1;

    public ConstructReportMessageUtility (
        ISlackUserCache slackUserCache, 
        IJiraConfigurationSection jiraConfigurationSection, 
        ISlackConfigurationSection slackConfigurationSection, 
        ISlackMessagingClient messagingClient, 
        IZenQuoteRandomClient zenQuoteRandomClient)
    {
        _slackUserCache = slackUserCache;
        _jiraConfigurationSection = jiraConfigurationSection;
        _slackConfigurationSection = slackConfigurationSection;
        _messagingClient = messagingClient;
        _zenQuoteRandomClient = zenQuoteRandomClient;
    }
    
    public void SetState(string channelId, string outputChannelId, int approvalRequired)
    {
        ChannelId = channelId;
        OutputChannelId = outputChannelId;
        ApprovalsRequired = approvalRequired;
        _stateIsSet = true;
    }

    public async Task SendPullRequestMessageThread(LinkExtractionResult link, IUserCollection approvedBy, IUserCollection codeOwners,
        GitlabApprovalsResponse approvals, string jiraTitleName, bool notApprovedByCodeOwners, bool containsJiraTicket)
    {
        if (!_stateIsSet)
            throw new ArgumentException("State is not set");
        
        var slackUser = await _slackUserCache.GetUser(link.Author);
        if (slackUser?.Ok == false)
            return;

        var jiraTicketNotifier = string.Empty;
        if (approvals.Title.GetJiraTicket(out var jiraTicket))
            jiraTicketNotifier =
                $"{SlackEmoji.Jira} {$"Jira {jiraTicket}".ToSlackLink(_jiraConfigurationSection.ConstructUrl(jiraTicket))} " 
                + Environment.NewLine;
                
        AddTitle($"*{(string.IsNullOrEmpty(jiraTitleName) ? approvals.Title : jiraTitleName)}*");
        
        if(approvedBy.Count > 0)
            AddContextApprovals(approvals);

        var archiveLink = SlackModelsExtensions.GetArchiveLink(
            _slackConfigurationSection.GetConfiguration()!.SlackOwner,
            ChannelId, 
            link.OriginalThreadId);

        var missingApprovals = ApprovalsRequired - approvals.ApprovedBy.Count;
        if (missingApprovals < 0) missingApprovals = 0;
        
        var codeOwnersMessage = string.Empty;
        
        if (notApprovedByCodeOwners)
        {
            codeOwnersMessage = Environment.NewLine + $"{SlackEmoji.TopHat} Missing code owners: ";
            foreach (var codeOwner in codeOwners.Where(x => approvedBy[x.Gitlab] == null))
            {
                codeOwnersMessage += codeOwner.Slack.ToSlackUserMention() + " ";
                // TODO send private message to slack (maybe?)
            }
        }

        var messageBody =
            $"Missing *{missingApprovals}* approvals! Submitted by: *{slackUser!.Profile.display_name}* {link.DaysDifference} days ago" +
            Environment.NewLine
            + $"{SlackEmoji.Thread} {"Original thread".ToSlackLink(archiveLink)} " + Environment.NewLine
            + jiraTicketNotifier
            + $"{SlackEmoji.PointRight} Pull request {link.ProjectName}/{link.PullRequestId} ".ToSlackLink(link.RawValue)
            + codeOwnersMessage;
        
        AddAuthorInformations(messageBody, slackUser);
        AddDivider();

        var message = await _messagingClient.PublishMessage(new()
        {
            Blocks = Blocks,
            ChannelId = OutputChannelId,
            UnfurLinks = false
        });
        
        if (!containsJiraTicket)
            await _messagingClient.PublishMessage(new()
            {
                Thread = message.ts,
                ChannelId = message.channel,
                Message = $"{link.Author.ToSlackUserMention()} title of the PR does not contain jira ticket reference. Can you please check and fix that?"
            });
                
        Blocks = new();
    }

    public async Task OnTheEnd(int messagesRead, int linksCount, int dayDifference, int approvals)
    {
        var quote = await _zenQuoteRandomClient.GetRandomQuote();

        var message = $"{GlobalConstants.Slack.HereMention} hello there! " +
                      $"Today I have read *{messagesRead}* messages and checked *{linksCount}* gitlab links for past *{dayDifference}* days, with criteria that MR has at least *{approvals}* approvals, " +
                      $"which means that you can at least resolve couple of those MR's, as I would not like going through same links each morning. Thanks!";
        
        if(quote != null)
            message += Environment.NewLine + Environment.NewLine + 
                       "Here is a random quote I have prepared for you, to motivate you at least a bit:" + Environment.NewLine + Environment.NewLine +
                       $"> {quote.Text}" + Environment.NewLine +
                       $" written by `{quote.Author}` ";

        await _messagingClient.PublishMessage(new()
        {
            Message = message,
            ChannelId = OutputChannelId,
            UnfurLinks = false,
        });        
    }


    private void AddTitle(string title)
    {
        Blocks.Add(new TextSection(title));
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

    private void AddDivider()
    {
        Blocks.Add(new Divider());
    }
    
    
}