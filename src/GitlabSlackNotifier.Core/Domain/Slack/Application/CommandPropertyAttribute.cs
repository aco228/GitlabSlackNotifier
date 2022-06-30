namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public class CommandPropertyAttribute : Attribute
{
    public string Name { get; set; }
    public bool Required { get; set; } = false;

    public CommandPropertyAttribute (string name)
    {
        Name = name;
    }
}