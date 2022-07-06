namespace GitlabSlackNotifier.Core.Domain.Utilities.Slack;

public class GetSlackMessageLinksUtilityResponse
{
    public List<LinkExtractionResult> Links { get; set; } = new();
    public int MessagesRead { get; set; } = 0;
    public int LinkCount => Links.Count;
}