using GitlabSlackNotifier.Core.Domain.Jira;

namespace GitlabSlackNotifier.Core.Services.Jira;

public interface IJiraIssueClient
{
    Task<JiraIssue?> GetIssue(string key);
}