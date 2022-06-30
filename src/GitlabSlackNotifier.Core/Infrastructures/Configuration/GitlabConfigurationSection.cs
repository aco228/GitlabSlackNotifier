using GitlabSlackNotifier.Core.Domain.Configuration;
using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Infrastructures.Configuration;

public interface IGitlabConfigurationSection : IConfigurationSection<GitlabConfiguration> { }

public class GitlabConfigurationSection : 
    ConfigurationSectionBase<GitlabConfiguration>, 
    IGitlabConfigurationSection
{
    public GitlabConfigurationSection(IConfigurationManager configurationManager) 
        : base("Gitlab", configurationManager)
    { }
}