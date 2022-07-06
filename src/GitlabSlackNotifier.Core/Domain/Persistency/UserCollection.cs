using GitlabSlackNotifier.Core.Services.Persistency;

namespace GitlabSlackNotifier.Core.Domain.Persistency;

public class UserCollection : List<User>, IUserCollection
{
    public User? this[string key]
    {
        get => this.FirstOrDefault(x => x.Slack.Equals(key)) 
               ?? this.FirstOrDefault(x => x.Gitlab.Equals(key)) 
               ?? null;
    }

    public void AddCollection(List<User> users)
    {
        foreach (var user in users)
            Add(user);
    }
}
