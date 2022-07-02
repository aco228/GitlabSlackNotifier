namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public abstract class CommandModelBase
{
    public virtual bool IsValid() => true;
}