using System.Collections.Concurrent;
using GitlabSlackNotifier.Core.Domain.Slack.Users;
using GitlabSlackNotifier.Core.Services.Slack;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackUserCache : ISlackUserCache
{
    private readonly ILogger<ISlackUserCache> _logger;
    private readonly ISlackUserClient _userClient;
    private ConcurrentDictionary<string, SlackUserResponse> _cachedUsers = new();

    public SlackUserCache (
        ILogger<ISlackUserCache> logger,
        ISlackUserClient userClient)
    {
        _logger = logger;
        _userClient = userClient;
    }


    public async Task<SlackUserResponse?> GetUser(string user)
    {
        if (_cachedUsers.TryGetValue(user, out var userResponse))
            return userResponse;

        _logger.LogInformation($"Caching info about slack user {user}");
        userResponse = await _userClient.GetUserInformations(user);

        if (userResponse == null)
            return null;

        _cachedUsers.TryAdd(user, userResponse);
        return userResponse;
    }
}