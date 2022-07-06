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

namespace GitlabSlackNotifier.Core.Infrastructures.Utilities.Slack;

public class ConstructReportMessageUtility : IConstructReportMessageUtility
{
    private bool _stateIsSet = false;
    private readonly ISlackUserCache _slackUserCache;
    private readonly IJiraConfigurationSection _jiraConfigurationSection;
    private readonly ISlackConfigurationSection _slackConfigurationSection;
    
    public List<BlockBase> Blocks { get; private set; } = new();
    public string ChannelId { get; set; }
    public int ApprovalsRequired { get; set; } = -1;
    
    

    public void SetState(string channelId, int approvalRequired)
    {
        ChannelId = channelId;
        ApprovalsRequired = approvalRequired;
        _stateIsSet = true;
    }

    public ConstructReportMessageUtility (
        ISlackUserCache slackUserCache, 
        IJiraConfigurationSection jiraConfigurationSection, 
        ISlackConfigurationSection slackConfigurationSection)
    {
        _slackUserCache = slackUserCache;
        _jiraConfigurationSection = jiraConfigurationSection;
        _slackConfigurationSection = slackConfigurationSection;

        Blocks.Add(new HeaderElement("MR's without enough approvals"));
        Blocks.Add(new Divider());
    }

    public async Task AddPullRequestSection(
        LinkExtractionResult link,
        IUserCollection approvedBy,
        IUserCollection codeOwners,
        GitlabApprovalsResponse approvals,
        bool notApprovedByCodeOwners)
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
                
        AddTitle($"*{approvals.Title}*");
        
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
    }

    public void OnTheEnd(int messagesRead, int linksCount, int dayDifference, int approvals)
    {
        var context = new ContextSection();
        context.Elements.Add(
            new TextElement($"I read {messagesRead} messages, " +
                            $"and checked {linksCount} links for past {dayDifference} days, " +
                            $"with criteria that MR has at least {approvals} approvals"));
        
        Blocks.Add(context);        
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