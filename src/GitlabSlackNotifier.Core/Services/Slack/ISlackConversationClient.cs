using GitlabSlackNotifier.Core.Domain.Slack.Channels;

namespace GitlabSlackNotifier.Core.Services.Slack;

public interface ISlackConversationClient
{
    Task<SingleConversationResponse> GetConversation(SingleConversationRequest request);
    Task<ConversationListResponse> GetConversationList(ConversationListRequest request);
    Task<ConversationMessagesResponse> GetMessages(ConversationMessagesRequest request);
}