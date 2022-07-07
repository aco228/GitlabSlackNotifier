using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Domain.Jira;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Services.Persistency;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public record ReportMergeRequest
{
    public LinkExtractionResult Link { get; set; }
    public IUserCollection ApprovedBy { get; set; }
    public IUserCollection CodeOwners { get; set; }
    public GitlabApprovalsResponse Approvals { get; set; }
    public JiraIssue? JiraIssue { get; set; } = null;
    public bool NotApprovedByCodeOwners { get; set; }
}