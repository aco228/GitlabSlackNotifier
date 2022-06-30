using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Services.Slack.Applications;

public interface ISlackCommandApplicationHandler
{
    Task RunCommand(SlackCommandRequest request, SlackCommandType commandType);
    bool GetCommand(string commandName, SlackCommandType commandType, out ISlackApplicationCommand? command);
    void AddCommand(Type command);
}