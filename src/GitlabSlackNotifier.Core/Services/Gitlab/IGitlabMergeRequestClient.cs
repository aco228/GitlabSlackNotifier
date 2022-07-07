using GitlabSlackNotifier.Core.Domain.Gitlab.MergeRequests;

namespace GitlabSlackNotifier.Core.Services.Gitlab;

public interface IGitlabMergeRequestClient
{
    Task<List<GitlabMergeRequestNote>> GetMergeRequestNotes(long projectId, int mergeRequestId);
}