using GitlabSlackNotifier.Core.Domain.Application.Commands;
using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;

namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public interface ISlackTestReadMessagesCommand : ISlackApplicationCommand { }

public class SlackTestReadMessagesCommand : SlackCommandComposeBase<ReadMessagesModel>, ISlackTestReadMessagesCommand
{
    public override string CommandName { get; } = "read_messages";
    public override SlackCommandType CommandType { get; } = SlackCommandType.Mention;

    private readonly ISlackMessagingClient _messagingClient;
    private readonly ISlackConversationClient _conversationClient;

    public SlackTestReadMessagesCommand(
        ISlackMessagingClient messagingClient,
        ISlackConversationClient conversationClient,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _messagingClient = messagingClient;
        _conversationClient = conversationClient;
    }
    
    protected override async Task Process(SlackCommandRequest request, ReadMessagesModel model)
    {
        var conversation = await _conversationClient.GetConversation(new ()
        {
            Channel = model.Channel
        });

        if (!conversation.Ok)
        {
            await ReportError(request, $"Could not get info about channel:{model.Channel}");
            return;
        }

        var messages = await _conversationClient.GetMessages(new()
        {
            Channel = model.Channel
        });
        
        if (!messages.Ok)
        {
            await ReportError(request, $"Could not read messages from channel:{model.Channel}");
            return;
        }

        await _messagingClient.PublishMessage(new()
        {
            ChannelId = request.Channel,
            Thread = request.MessageThread,
            Message = $"You sent this channel {conversation.Channel.Name}, with latest message = {messages.Messages.FirstOrDefault().Text}"
        });
    }
}