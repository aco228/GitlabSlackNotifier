using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackConversationClient : ISlackConversationClient
{
    private readonly ISlackHttpClient _client;
    
    public SlackConversationClient (ISlackHttpClient httpClient)
    {
        _client = httpClient;
    }

    public Task<SingleConversationResponse> GetConversation(SingleConversationRequest request)
        => _client.Get<SingleConversationResponse>("conversations.info", request);

    public Task<ConversationListResponse> GetConversationList(ConversationListRequest request)
        => _client.Get<ConversationListResponse>("conversations.list", request);

    public Task<ConversationMessagesResponse> GetMessages(ConversationMessagesRequest request)
        => _client.Get<ConversationMessagesResponse>("conversations.history", request);
}