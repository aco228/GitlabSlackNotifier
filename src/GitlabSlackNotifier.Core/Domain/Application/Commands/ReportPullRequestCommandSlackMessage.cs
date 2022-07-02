using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;
using GitlabSlackNotifier.Core.Domain.Slack.Users;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public class ReportPullRequestCommandSlackMessage
{

    public ReportPullRequestCommandModel Model { get; private set; }
    public List<BlockBase> Blocks { get; private set; }
    
    public ReportPullRequestCommandSlackMessage(ReportPullRequestCommandModel model)
    {
        Model = model;
        Blocks = new List<BlockBase>();
        Blocks.Add(new HeaderElement("MR's without enough approvals"));
        Blocks.Add(new TextSection($"Gone {model.DurationPeriod.GetDayDifference()} days in past."));
        Blocks.Add(new TextSection($"Criteria is that MR has at least {model.Approvals} approvals"));
        Blocks.Add(new Divider());
    }

    public void AddTitle(string title)
    {
        Blocks.Add(new TextSection(title));
    }

    public void AddContextApprovals(
        GitlabApprovalsResponse approvals)
    {
        var contextSection = new ContextSection();
        contextSection.Elements.Add(new TextElement("Approved by: "));
        
        foreach (var approval in approvals.ApprovedBy)
            contextSection.Elements.Add(new ImageElement(approval.User.PictureUrl, approval.User.Username));
        
        Blocks.Add(contextSection);
    }

    public void AddAuthorInformations(string text, SlackUserResponse user)
    {
        Blocks.Add(new ImageSection(text, user.Profile.image_192, user.Profile.display_name));
    }

    public void AddDivider()
    {
        Blocks.Add(new Divider());
    }
}