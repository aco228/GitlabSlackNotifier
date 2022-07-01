using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Domain.Slack.Channels;

namespace GitlabSlackNotifier.Core.Services.LinkExtraction;

public interface ISlackLinkExtractor
{
    public IEnumerable<LinkExtractionResult> ExtractLinks(SlackMessageResponse input);
}