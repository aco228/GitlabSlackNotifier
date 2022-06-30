using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;

namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public interface ISlackTestReadMessagesCommand : ISlackApplicationCommand { }

public class SlackTestReadMessagesCommand : SlackCommandComposeBase<ReadMessagesModel>, ISlackTestReadMessagesCommand
{
    public override string CommandName { get; } = "read_messages";
    public override SlackCommandType CommandType { get; } = SlackCommandType.Mention;

    private readonly ISlackMessagingClient _messagingClient;

    public SlackTestReadMessagesCommand(
        ISlackMessagingClient messagingClient,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _messagingClient = messagingClient;
    }
    
    protected override Task Process(SlackCommandRequest request, ReadMessagesModel model)
    {
        return _messagingClient.PublishMessage(new()
        {
            ChannelId = request.Channel,
            Thread = request.MessageThread,
            Message = $"You sent this channel {model.ChannelName}"
        });
    }
}