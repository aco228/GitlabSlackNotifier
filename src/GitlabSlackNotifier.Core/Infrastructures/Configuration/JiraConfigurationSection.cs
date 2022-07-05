using GitlabSlackNotifier.Core.Domain.Configuration;
using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Infrastructures.Configuration;

public interface IJiraConfigurationSection : IConfigurationSection<JiraConfiguration>
{
    string ConstructUrl(string jiraTicket);
}

public class JiraConfigurationSection :
    ConfigurationSectionBase<JiraConfiguration>, 
    IJiraConfigurationSection
{
    public JiraConfigurationSection (IConfigurationManager configurationManager) 
        : base("Jira", configurationManager)
    {
    }

    public string ConstructUrl(string jiraTicket)
    {
        var configuration = GetConfiguration()!;
        return configuration.UrlPlaceholder
            .Replace(GlobalConstants.Jira.JiraOwnerPlaceHolder, configuration.Owner)
            .Replace(GlobalConstants.Jira.JiraIssuePlaceholder, jiraTicket);
    }
}