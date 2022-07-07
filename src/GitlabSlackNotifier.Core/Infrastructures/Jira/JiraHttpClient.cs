using System.Text;
using Aco228.SimpleHttpClient;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Services.Jira;

namespace GitlabSlackNotifier.Core.Infrastructures.Jira;

public class JiraHttpClient : RequestClient, IJiraHttpClient
{
    public JiraHttpClient (IJiraConfigurationSection jiraConfigurationSection)
    {
        var configuration = jiraConfigurationSection.GetConfiguration();
        if (configuration?.AreCriticalPropertiesValid() == false)
            throw new ArgumentException("Jira configuration is not set!");
        
        SetBaseString(configuration!.ApiBaseUrl);
        
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{configuration.UserEmail}:{configuration.UserToken}"));
        AddDefaultHeader("Authorization", $"Basic {token}");
    }
}