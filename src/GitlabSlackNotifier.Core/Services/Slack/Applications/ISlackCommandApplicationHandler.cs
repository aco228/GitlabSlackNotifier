using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Services.Slack.Applications;

public interface ISlackCommandApplicationHandler
{
    Task RunCommand(SlackCommandRequest request);
    bool GetCommand(string commandName, out ISlackApplicationCommand? command);
    void AddCommand(Type command);
}