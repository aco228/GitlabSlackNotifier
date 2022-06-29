using GitlabSlackNotifier.Core.Domain.Slack;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackMessagingClient : ISlackMessagingClient
{
    private readonly ISlackHttpClient _client;
    
    public SlackMessagingClient (ISlackHttpClient httpClient)
    {
        _client = httpClient;
    }
    
    public Task<PublishMessageResponse> PublishMessage(PublishMessageRequest request)
        => _client.Post<PublishMessageResponse>("chat.postMessage", request);
}