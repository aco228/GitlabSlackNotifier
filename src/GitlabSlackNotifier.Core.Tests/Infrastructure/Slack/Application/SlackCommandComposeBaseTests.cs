using GitlabSlackNotifier.Core.Domain.Slack;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Tests.Mocks;
using GitlabSlackNotifier.Core.Tests.Mocks.Infrastructure.Slack.Application;
using Microsoft.Extensions.Logging;
using Moq;

namespace GitlabSlackNotifier.Core.Tests.Infrastructure.Slack.Application;

public class SlackCommandComposeBaseTests
{
    private Mock<IServiceProvider> _serviceProviderMock = new ();
    private Mock<ISlackMessagingClient> _slackMessagingClientMock = new ();
    private CollectResultMockService _collectResultService = new();
    private Mock<ILogger<DummyCommandComposeModel>> _logger = new();


    [Fact]
    public async Task Should_Parse_Argument_To_ModelObject()
    {
        var service = GetService();
        var request = new SlackCommandRequest
        {
            Channel = "channel",
            Text = "text arg1 arg2 arg3",
            User = "user",
            MessageThread = "ts"
        };
        var arguments = new [] {"name=Aleksandar"};

        await service.Process(request, arguments);
        
        _slackMessagingClientMock.Verify(x => x.PublishMessage(It.IsAny<PublishMessageRequest>()), Times.Never);
        
        Assert.Equal(2, _collectResultService.Objects.Count);
        Assert.Equal(request, _collectResultService.Objects[0]);
        
        var model = _collectResultService.Objects[1] as DummyCommandComposeModel;
        Assert.NotNull(model);
        
        Assert.Equal("Aleksandar", model.Name);
        Assert.Equal(false, model.IsRokoTrue);
        Assert.Equal(default(int), model.Count);
        Assert.Null(model.Number);
    }
    
    [Fact]
    public async Task Should_Parse_Optional_Argument()
    {
        var service = GetService();
        var request = new SlackCommandRequest()
        {
            Channel = "channel",
            Text = "text arg1 arg2 arg3",
            User = "user",
            MessageThread = "ts"
        };
        var arguments = new [] {"name=Aleksandar", "count=15", "roko=true"};

        await service.Process(request, arguments);
        
        _slackMessagingClientMock.Verify(x => x.PublishMessage(It.IsAny<PublishMessageRequest>()), Times.Never);
        
        Assert.Equal(2, _collectResultService.Objects.Count);
        Assert.Equal(request, _collectResultService.Objects[0]);
        
        var model = _collectResultService.Objects[1] as DummyCommandComposeModel;
        Assert.NotNull(model);
        
        Assert.Equal("Aleksandar", model.Name);
        Assert.Equal(15, model.Count);
        Assert.Equal(true, model.IsRokoTrue);
        Assert.Null(model.Number);
    }
    
    
    [Fact]
    public async Task Should_Parse_Nullable_Argument()
    {
        var service = GetService();
        var request = new SlackCommandRequest
        {
            Channel = "channel",
            Text = "text arg1 arg2 arg3",
            User = "user",
            MessageThread = "ts"
        };
        var arguments = new [] {"number=13", "name=Aleksandar", "count=15", "roko=true"};

        await service.Process(request, arguments);
        
        _slackMessagingClientMock.Verify(x => x.PublishMessage(It.IsAny<PublishMessageRequest>()), Times.Never);
        
        Assert.Equal(2, _collectResultService.Objects.Count);
        Assert.Equal(request, _collectResultService.Objects[0]);
        
        var model = _collectResultService.Objects[1] as DummyCommandComposeModel;
        Assert.NotNull(model);
        
        Assert.Equal("Aleksandar", model.Name);
        Assert.Equal(15, model.Count);
        Assert.Equal(true, model.IsRokoTrue);
        Assert.NotNull(model.Number);
        Assert.Equal(13, model.Number.Value);
    }
    
    
    [Fact]
    public async Task Should_Fail_If_RequiredArgument_Is_Missing()
    {
        var service = GetService();
        var request = new SlackCommandRequest()
        {
            Channel = "channel",
            Text = "text arg1 arg2 arg3",
            User = "user",
            MessageThread = "ts"
        };
        var arguments = new [] {"count=15"};

        await service.Process(request, arguments);
        
        _slackMessagingClientMock.Verify(x => x.PublishMessage(It.IsAny<PublishMessageRequest>()), Times.Once);
        
        // because it should not be called
        Assert.Equal(0, _collectResultService.Objects.Count);
    }


    private DummySlackCommandCompose GetService()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _slackMessagingClientMock = new Mock<ISlackMessagingClient>();
        _collectResultService = new();

        _serviceProviderMock.Setup(x => 
                x.GetService(
                    It.Is<Type>(s => s == typeof(ISlackMessagingClient))))
            .Returns(_slackMessagingClientMock.Object);
        
        _serviceProviderMock.Setup(x => 
                x.GetService(
                    It.Is<Type>(s => s == typeof(ILogger<DummyCommandComposeModel>))))
            .Returns(_logger.Object);
        
        var service = new DummySlackCommandCompose(_collectResultService, _serviceProviderMock.Object);

        return service;
    }
}