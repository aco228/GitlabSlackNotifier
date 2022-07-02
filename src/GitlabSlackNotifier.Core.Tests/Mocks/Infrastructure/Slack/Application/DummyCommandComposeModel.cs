using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Tests.Mocks.Infrastructure.Slack.Application;

public class DummyCommandComposeModel : CommandModelBase
{
    
    
    [CommandProperty("number", Required = false)]
    public int? Number { get; set; }
    
    [CommandProperty("name", Required = true)]
    public string Name { get; set; }
    
    [CommandProperty("count", Required = false)]
    public int Count { get; set; }
    
    [CommandProperty("roko", Required = false)]
    public bool IsRokoTrue { get; set; }

    public override bool IsValid()
    {
        return true;
    }
}