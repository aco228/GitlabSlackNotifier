using GitlabSlackNotifier.Core.Domain.Configuration;
using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Infrastructures.Configuration;

public interface ISlackConfigurationSection : IConfigurationSection<SlackConfiguration> { }
public class SlackConfigurationSection : 
    ConfigurationSectionBase<SlackConfiguration>, 
    ISlackConfigurationSection
{
    public SlackConfigurationSection(IConfigurationManager configurationManager) 
        : base("Slack", configurationManager)
    { }
}