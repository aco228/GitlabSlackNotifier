using GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;

namespace GitlabSlackNotifier.Core.Domain;

public static class SlackModelsExtensions
{
    public static bool IsValid(this EventUponMessageRequestModel? input)
        => input != null && input.Event != null;

    public static string GetArchiveLink(string slackOwner, string channel, string originalThread)
        => $"https://{slackOwner}/archives/{channel}/p{originalThread.Replace(".", "")}";
}