namespace GitlabSlackNotifier.Core.Services.Slack;

public interface ISlackDefaultChannel
{
    Task SendMessage(string message);
}