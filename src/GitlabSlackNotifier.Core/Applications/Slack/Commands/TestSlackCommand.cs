using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Applications.Slack.Commands;

public interface ITestSlackCommand : IHelloSlackCommand { }
public class TestSlackCommand : ITestSlackCommand
{
    public string CommandName => "test";

    private readonly ISlackDefaultChannel _defaultChannel;

    public TestSlackCommand (ISlackDefaultChannel defaultChannel)
    {
        _defaultChannel = defaultChannel;
    }
    
    public Task Process(SlackCommandRequest request, string[] arguments)
    {
        return _defaultChannel.SendMessage("Test is working");
    }
}