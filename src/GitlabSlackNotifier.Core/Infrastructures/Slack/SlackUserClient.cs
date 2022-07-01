using GitlabSlackNotifier.Core.Domain.Slack.Users;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackUserClient : ISlackUserClient
{
    private readonly ISlackHttpClient _client;

    public SlackUserClient (ISlackHttpClient client)
    {
        _client = client;
    }

    public Task<SlackUserResponse?> GetUserInformations(string user)
        => _client.Get<SlackUserResponse?>($"users.profile.get?user={user}");

}