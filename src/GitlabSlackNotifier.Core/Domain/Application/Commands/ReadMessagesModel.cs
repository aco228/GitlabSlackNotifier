using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public class ReadMessagesModel : CommandModelBase
{
    [CommandProperty("channel", Required = true)]
    public string Channel { get; set; }

}