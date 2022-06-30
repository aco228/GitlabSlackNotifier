using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Tests.Mocks.Infrastructure.Slack.Application;

public class DummyCommandComposeModel
{
    [CommandProperty("name", Required = true)]
    public string Name { get; set; }
    
    [CommandProperty("count", Required = false)]
    public int Count { get; set; }
    
    [CommandProperty("roko", Required = false)]
    public bool IsRokoTrue { get; set; }
}