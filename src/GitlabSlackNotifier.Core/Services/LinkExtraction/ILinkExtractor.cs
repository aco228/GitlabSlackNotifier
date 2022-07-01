using GitlabSlackNotifier.Core.Domain.LinkExtraction;

namespace GitlabSlackNotifier.Core.Services.LinkExtraction;

public interface ILinkExtractor
{
    public IEnumerable<LinkExtractionResult> ExtractLinks(string input);
}