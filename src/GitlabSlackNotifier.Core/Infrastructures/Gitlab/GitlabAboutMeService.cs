using GitlabSlackNotifier.Core.Domain.Gitlab.AboutMeModels;
using GitlabSlackNotifier.Core.Services.Gitlab;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabAboutMeService : IGitlabAboutMeService
{
    private readonly IGitlabHttpClient _client;
    
    public GitlabAboutMeService (IGitlabHttpClient httpClient)
    {
        _client = httpClient;
    }

    public Task<GitlabAboutMeResponse> GetAboutMeInfo()
        => _client.Get<GitlabAboutMeResponse>("user");
}