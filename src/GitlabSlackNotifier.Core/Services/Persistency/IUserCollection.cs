using GitlabSlackNotifier.Core.Domain.Persistency;

namespace GitlabSlackNotifier.Core.Services.Persistency;

public interface IUserCollection: IList<User>
{
    User? this[string key] { get; }
    void AddCollection(List<User> users);
}