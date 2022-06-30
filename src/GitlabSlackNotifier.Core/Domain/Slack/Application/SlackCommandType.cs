namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

[Flags]
public enum SlackCommandType
{
    Mention = 1,
    Command = 2,
}