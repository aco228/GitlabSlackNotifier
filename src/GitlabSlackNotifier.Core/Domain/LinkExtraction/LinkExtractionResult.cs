namespace GitlabSlackNotifier.Core.Domain.LinkExtraction;

public record LinkExtractionResult
{
    public string ProjectName { get; set; }
    public int PullRequestId { get; set; }
    public string RawValue { get; set; }
    public string Author { get; set; }
    public string OriginalThreadId { get; set; }
    public DateTime Created { get; set; }
}