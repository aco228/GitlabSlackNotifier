using GitlabSlackNotifier.Core.Domain.Gitlab.AboutMeModels;

namespace GitlabSlackNotifier.Core.Services.Gitlab;

public interface IGitlabAboutMeService
{
    Task<GitlabAboutMeResponse> GetAboutMeInfo();
}