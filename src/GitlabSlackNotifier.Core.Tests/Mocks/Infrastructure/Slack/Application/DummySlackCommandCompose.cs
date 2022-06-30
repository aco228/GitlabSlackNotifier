using GitlabSlackNotifier.Core.Domain.Slack.Application;

namespace GitlabSlackNotifier.Core.Tests.Mocks.Infrastructure.Slack.Application;

public class DummySlackCommandCompose : SlackCommandComposeBase<DummyCommandComposeModel>
{
    private readonly CollectResultMockService _collectResultMockService;

    public DummySlackCommandCompose(
        CollectResultMockService collectResultMockService,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _collectResultMockService = collectResultMockService;
    }

    public override string CommandName { get; } = "dummy";
    public override SlackCommandType CommandType { get; } = SlackCommandType.Mention;
    
    protected override Task Process(SlackCommandRequest request, DummyCommandComposeModel model)
    {
        _collectResultMockService.Collect(request,model);
        return Task.FromResult(true);
    }
}