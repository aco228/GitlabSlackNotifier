using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;

namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public abstract class SlackCommandBase : ISlackApplicationCommand
{
    private readonly IServiceProvider _serviceProvider;
    
    public abstract string CommandName { get; }
    public abstract SlackCommandType CommandType { get; }
    public abstract Task Process(SlackCommandRequest request, string[] arguments);

    public SlackCommandBase (IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected Task ReportBackMessage(SlackCommandRequest request, string errorMessage)
    {
        if (!string.IsNullOrEmpty(request.MessageThread))
        {
            var messageClient = _serviceProvider.GetService(typeof(ISlackMessagingClient)) as ISlackMessagingClient;
            return messageClient.PublishMessage(new()
            {
                ChannelId = request.Channel,
                Thread = request.MessageThread,
                Message = errorMessage
            });
        }

        var defaultChannel = _serviceProvider.GetService(typeof(ISlackDefaultChannel)) as ISlackDefaultChannel;
        return defaultChannel.SendMessage(errorMessage);
    }
}