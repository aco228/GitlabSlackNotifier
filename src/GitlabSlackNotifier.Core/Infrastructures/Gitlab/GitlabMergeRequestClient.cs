using GitlabSlackNotifier.Core.Domain.Gitlab.MergeRequests;
using GitlabSlackNotifier.Core.Services.Gitlab;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabMergeRequestClient : IGitlabMergeRequestClient
{
    private readonly IGitlabHttpClient _client;
    
    public GitlabMergeRequestClient (IGitlabHttpClient client)
    {
        _client = client;
    }

    public Task<List<GitlabMergeRequestNote>> GetMergeRequestNotes(long projectId, int mergeRequestId)
        => _client.Get<List<GitlabMergeRequestNote>>($"projects/{projectId}/merge_requests/{mergeRequestId}/notes");
}