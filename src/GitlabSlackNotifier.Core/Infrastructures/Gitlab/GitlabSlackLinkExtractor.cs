using System.Text.RegularExpressions;
using GitlabSlackNotifier.Core.Domain.LinkExtraction;
using GitlabSlackNotifier.Core.Domain.Slack.Channels;
using GitlabSlackNotifier.Core.Services.LinkExtraction;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public interface IGitlabSlackLinkExtractor : ISlackLinkExtractor { }
public class GitlabSlackLinkExtractor : IGitlabSlackLinkExtractor
{
    private static string RegexQuery = @"<(https:\/\/gitlab\.com\/(.+)\/-\/merge_requests\/([0-9]+))>?";
    
    public IEnumerable<LinkExtractionResult> ExtractLinks(SlackMessageResponse input)
    {
        var regex = new Regex(RegexQuery);
        var matches = regex.Matches(input.Text);
        if (matches.Count == 0)
            yield break;

        foreach (Match match in matches)
        {
            var entry = new LinkExtractionResult { RawValue = match.Value};
            if (match.Groups.Count >= 4 && int.TryParse(match.Groups[3].Value, out int pullRequestId))
            {
                entry.ProjectName = match.Groups[2].Value;
                entry.PullRequestId = pullRequestId;
                entry.OriginalThreadId = match.Groups[1].Value;
                entry.Author = input.User;
                entry.Created = input.GetDate()!.Value;
                yield return entry;
            }
        }
    }
}