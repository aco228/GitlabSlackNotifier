using System.Text.RegularExpressions;
using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Services.LinkExtraction;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public interface IGitlabLinkExtractor : ILinkExtractor { }
public class GitlabLinkExtractor : IGitlabLinkExtractor
{
    private static string RegexQuery = @"<(https:\/\/gitlab\.com\/(.+)\/-\/merge_requests\/([0-9]+))>?";
    
    public IEnumerable<LinkExtractionResult> ExtractLinks(string input)
    {
        var regex = new Regex(RegexQuery);
        var matches = regex.Matches(input);
        if (matches.Count == 0)
            yield break;

        foreach (Match match in matches)
        {
            var entry = new LinkExtractionResult { RawValue = match.Value};
            if (match.Groups.Count >= 4 && int.TryParse(match.Groups[3].Value, out int intval))
            {
                entry.Name = match.Groups[2].Value;
                entry.Value = intval;
                yield return entry;
            }
        }
    }
}