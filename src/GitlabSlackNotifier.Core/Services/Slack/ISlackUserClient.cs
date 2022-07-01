using GitlabSlackNotifier.Core.Domain.Slack.Users;

namespace GitlabSlackNotifier.Core.Services.Slack;

public interface ISlackUserClient
{
    Task<SlackUserResponse?> GetUserInformations(string user);
}