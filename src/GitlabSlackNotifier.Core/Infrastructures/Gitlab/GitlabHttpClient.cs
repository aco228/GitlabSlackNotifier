using Aco228.SimpleHttpClient;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Services.Gitlab;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabHttpClient : RequestClient, IGitlabHttpClient
{
    private const string TokenHeaderName = "PRIVATE-TOKEN";
   
    public GitlabHttpClient (IGitlabConfigurationSection gitlabConfigurationSection)
    {
        var configuration = gitlabConfigurationSection.GetConfiguration();

        if (configuration == null || configuration?.AreCriticalPropertiesValid() == false)
            throw new ArgumentException("Gitlab access token is not defined in appsettings or env");
        
        AddDefaultHeader(TokenHeaderName, configuration!.AccessToken);
        SetBaseString(configuration!.BaseApiUrl);
    }
}