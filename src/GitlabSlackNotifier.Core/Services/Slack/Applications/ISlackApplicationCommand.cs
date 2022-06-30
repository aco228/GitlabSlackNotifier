using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Services.Slack.Applications;

public interface ISlackApplicationCommand
{
    string CommandName { get; }
    Task Process(SlackCommandRequest request, string[] arguments);
}