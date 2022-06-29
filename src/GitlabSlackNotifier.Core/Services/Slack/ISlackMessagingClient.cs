using GitlabSlackNotifier.Core.Domain.Slack;

namespace GitlabSlackNotifier.Core.Services.Slack;

public interface ISlackMessagingClient
{
    Task<PublishMessageResponse> PublishMessage(PublishMessageRequest request);
}