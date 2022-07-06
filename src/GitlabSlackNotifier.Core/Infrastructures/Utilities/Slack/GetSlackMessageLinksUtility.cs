using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;
using GitlabSlackNotifier.Core.Infrastructures.Utilities.Gitlab;
using GitlabSlackNotifier.Core.Services.LinkExtraction;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Utilities.Slack;

namespace GitlabSlackNotifier.Core.Infrastructures.Utilities.Slack;

public class GetSlackMessageLinksUtility : IGetSlackMessageLinkUtility
{
    private readonly ISlackConversationClient _conversationClient;
    private readonly IGitlabSlackLinkExtractorUtility _slackLinkExtractorUtility;
    
    public GetSlackMessageLinksUtility (
        ISlackConversationClient conversationClient, 
        IGitlabSlackLinkExtractorUtility slackLinkExtractorUtility)
    {
        _conversationClient = conversationClient;
        _slackLinkExtractorUtility = slackLinkExtractorUtility;
    }

    public async Task<GetSlackMessageLinksUtilityResponse> GetLinksFromSlackChannel(
        string channelId,
        DurationPeriod durationPeriod,
        DurationPeriod skipPeriod)
    {
        var result = new Dictionary<string, LinkExtractionResult>();
        
        var messageCount = 0;
        var conversationRequest = new ConversationMessagesRequest { Channel = channelId };
        
        for (;;)
        {
            var messages = await _conversationClient.GetMessages(conversationRequest);

            if (!messages.Ok
                || messages.Messages.Count == 0)
                break;

            var timeError = false;
            foreach (var msg in messages.Messages)
            {
                ++messageCount;
                if (!msg.GetDate(out var msgDate))
                {
                    timeError = true;
                    break;
                }
                
                if (skipPeriod!.IsDateInPeriod(msgDate))
                    continue;
                
                if (!durationPeriod!.IsDateInPeriod(msgDate))
                {
                    timeError = true;
                    break;
                }

                foreach (var link in _slackLinkExtractorUtility.ExtractLinks(msg))
                    if (!result.ContainsKey(link.Key))
                        result.Add(link.Key, link);
            }

            if (timeError || !messages.HasMore)
                break;

            conversationRequest.LatestMessageThread = messages.Messages.First().MessageThread;
            conversationRequest.OldestMessageThread = messages.Messages.Last().MessageThread;
        }

        var response = new GetSlackMessageLinksUtilityResponse
        {
            MessagesRead = messageCount,
            Links = result.Select(x => x.Value).ToList()
        };

        return response;
    }
}