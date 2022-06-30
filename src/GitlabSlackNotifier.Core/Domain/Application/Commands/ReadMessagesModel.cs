using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public class ReadMessagesModel
{
    [CommandProperty("channelName", Required = true)]
    public string ChannelName { get; set; }
}