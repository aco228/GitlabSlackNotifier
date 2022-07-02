using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;

namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public abstract class SlackCommandBase : ISlackApplicationCommand
{
    public abstract string CommandName { get; }
    public abstract SlackCommandType CommandType { get; }
    protected IServiceProvider ServiceProvider { get; private set; }
    
    public abstract Task Process(SlackCommandRequest request, string[] arguments);

    public SlackCommandBase (IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    internal Task ReportBackMessage(SlackCommandRequest request, string errorMessage)
    {
        if (!string.IsNullOrEmpty(request.MessageThread))
        {
            var messageClient = ServiceProvider.GetService(typeof(ISlackMessagingClient)) as ISlackMessagingClient;
            return messageClient!.PublishMessage(new()
            {
                ChannelId = request.Channel,
                Thread = request.MessageThread,
                Message = errorMessage
            });
        }

        var defaultChannel = ServiceProvider.GetService(typeof(ISlackDefaultChannel)) as ISlackDefaultChannel;
        return defaultChannel!.SendMessage(errorMessage);
    }
}