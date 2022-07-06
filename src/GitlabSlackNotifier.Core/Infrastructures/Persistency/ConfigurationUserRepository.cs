using GitlabSlackNotifier.Core.Domain.Persistency;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Services.Persistency;

namespace GitlabSlackNotifier.Core.Infrastructures.Persistency;

public class ConfigurationUserRepository : IUserRepository
{
    private readonly IUserCollection _users;
    
    public ConfigurationUserRepository (IUsersConfigurationSection usersConfigurationSection)
    {
        _users = new UserCollection();
        _users.AddCollection(usersConfigurationSection.GetConfiguration()!.Db);
    }

    public IUserCollection GetAllUsers()
        => _users;

    public User? GetUserIdentifier(string identifier)
        => _users[identifier];
}