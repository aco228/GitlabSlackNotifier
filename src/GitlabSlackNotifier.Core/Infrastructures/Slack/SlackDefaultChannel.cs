using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackDefaultChannel : ISlackDefaultChannel
{
    private readonly ISlackMessagingClient _messagingClient;
    private readonly string _defaultChannelId;
    
    public SlackDefaultChannel (
        ISlackConfigurationSection slackConfiguration,
        ISlackMessagingClient messagingClient)
    {
        var configuration = slackConfiguration.GetConfiguration();
        _defaultChannelId = configuration?.MainChannelId ?? string.Empty;
        if (string.IsNullOrEmpty(_defaultChannelId))
            throw new ArgumentException("Slack:MainChannelId is not set in appsettings");

        _messagingClient = messagingClient;
    }

    public Task SendMessage(string message)
        => _messagingClient.PublishMessage(new ()
        {
            ChannelId = _defaultChannelId,
            Message = message,
        });
}