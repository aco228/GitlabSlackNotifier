using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Applications.Slack.Commands;

public interface ITestSlackCommand : IHelloSlackCommand { }
public class TestSlackCommand : ITestSlackCommand
{
    public string CommandName => "test";
    public SlackCommandType CommandType { get; } = SlackCommandType.Command | SlackCommandType.Mention;

    private readonly ISlackDefaultChannel _defaultChannel;
    private readonly IGitlabAboutMeService _aboutMeService;

    public TestSlackCommand (
        ISlackDefaultChannel defaultChannel,
        IGitlabAboutMeService gitlabAboutMeService)
    {
        _defaultChannel = defaultChannel;
        _aboutMeService = gitlabAboutMeService;
    }
    
    public async Task Process(SlackCommandRequest request, string[] arguments)
    {
        var aboutMeResponse = await _aboutMeService.GetAboutMeInfo();
        await _defaultChannel.SendMessage($"Test is working with gitlab integration under {aboutMeResponse.Username}");
    }
}