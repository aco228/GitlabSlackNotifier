namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public record SlackCommandRequest
{
    public string User { get; set; }
    public string MessageThread { get; set; }
    public string Channel { get; set; }
    public string Text { get; set; }
};