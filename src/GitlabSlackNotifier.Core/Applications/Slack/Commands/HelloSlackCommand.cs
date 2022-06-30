using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;

namespace GitlabSlackNotifier.Core.Applications.Slack.Commands;

public interface IHelloSlackCommand : ISlackApplicationCommand {}
public class HelloSlackCommand : IHelloSlackCommand
{
    public string CommandName => "hello";
    public SlackCommandType CommandType { get; } = SlackCommandType.Command | SlackCommandType.Mention;

    private readonly ISlackMessagingClient _messagingClient;

    public HelloSlackCommand (ISlackMessagingClient messagingClient)
    {
        _messagingClient = messagingClient;
    }
    
    public Task Process(SlackCommandRequest request, string[] arguments)
    {
        return _messagingClient.PublishMessage(new ()
        {
            ChannelId = request.Channel,
            Thread = request.MessageThread,
            Message = $"Hello {request.User}, you sent this: " + string.Join(' ', arguments)
        });
    }
}