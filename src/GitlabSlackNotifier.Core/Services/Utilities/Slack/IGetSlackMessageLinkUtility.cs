using GitlabSlackNotifier.Core.Domain.Utilities.Slack;

namespace GitlabSlackNotifier.Core.Services.Utilities.Slack;

public interface IGetSlackMessageLinkUtility
{
    Task<GetSlackMessageLinksUtilityResponse> GetLinksFromSlackChannel(
        string channelId,
        DurationPeriod durationPeriod,
        DurationPeriod skipPeriod);
}