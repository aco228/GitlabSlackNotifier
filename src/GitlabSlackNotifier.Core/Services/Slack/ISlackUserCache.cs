using GitlabSlackNotifier.Core.Domain.Slack.Users;

namespace GitlabSlackNotifier.Core.Services.Slack;

public interface ISlackUserCache
{
    Task<SlackUserResponse?> GetUser(string user);
}