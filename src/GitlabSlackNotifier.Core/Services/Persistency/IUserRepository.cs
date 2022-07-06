using GitlabSlackNotifier.Core.Domain.Persistency;

namespace GitlabSlackNotifier.Core.Services.Persistency;

public interface IUserRepository
{
    IUserCollection GetAllUsers();
    User? GetUserIdentifier(string identifier);
}