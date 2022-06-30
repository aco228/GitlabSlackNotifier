using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Domain.Configuration;

public class GitlabConfiguration : IConfigurationModel
{
    public string BaseApiUrl { get; set; }
    public string AccessToken { get; set; }


    public bool AreCriticalPropertiesValid()
        => !string.IsNullOrEmpty(BaseApiUrl) && !string.IsNullOrEmpty(AccessToken);
}