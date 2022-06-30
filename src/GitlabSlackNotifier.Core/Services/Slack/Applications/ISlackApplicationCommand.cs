using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Services.Slack.Applications;

public interface ISlackApplicationCommand
{
    string CommandName { get; }
    SlackCommandType CommandType { get; }
    Task Process(SlackCommandRequest request, string[] arguments);
}