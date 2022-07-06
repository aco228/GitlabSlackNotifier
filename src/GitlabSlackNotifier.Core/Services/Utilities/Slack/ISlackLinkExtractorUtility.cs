using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Domain.Utilities.Slack;

namespace GitlabSlackNotifier.Core.Services.LinkExtraction;

public interface ISlackLinkExtractorUtility
{
    public IEnumerable<LinkExtractionResult> ExtractLinks(SlackMessageResponse input);
}