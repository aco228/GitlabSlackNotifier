using Aco228.JsonToken;
using GitlabSlackNotifier.Core.Domain.Jira;
using GitlabSlackNotifier.Core.Services.Jira;

namespace GitlabSlackNotifier.Core.Infrastructures.Jira;

public class JiraIssueClient : IJiraIssueClient
{
    private readonly IJiraHttpClient _client;

    public JiraIssueClient(IJiraHttpClient client)
    {
        _client = client;
    }

    public async Task<JiraIssue?> GetIssue(string key)
    {
        var response = await _client.Get($"issue/{key}");
        return JsonTokenConverter.Convert<JiraIssue>(response);
    }
}