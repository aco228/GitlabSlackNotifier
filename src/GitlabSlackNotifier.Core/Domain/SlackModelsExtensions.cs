using GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;

namespace GitlabSlackNotifier.Core.Domain;

public static class SlackModelsExtensions
{
    public static bool IsValid(this EventUponMessageRequestModel? input)
        => input != null && input.Event != null;
}