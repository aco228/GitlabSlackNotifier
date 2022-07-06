using GitlabSlackNotifier.Core.Domain.Configuration;
using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Infrastructures.Configuration;

public interface IUsersConfigurationSection : IConfigurationSection<UsersConfiguration> { }
public class UsersConfigurationSection : 
    ConfigurationSectionBase<UsersConfiguration>, 
    IUsersConfigurationSection
{
    public UsersConfigurationSection(IConfigurationManager configurationManager) 
        : base("Users", configurationManager)
    { }
}