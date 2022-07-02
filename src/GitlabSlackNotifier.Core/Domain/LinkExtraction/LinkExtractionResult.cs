namespace GitlabSlackNotifier.Core.Domain.LinkExtraction;

public record LinkExtractionResult
{
    private int? _daysDiffence = null;
    
    public string ProjectName { get; set; }
    public int PullRequestId { get; set; }
    public string RawValue { get; set; }
    public string Author { get; set; }
    public string OriginalThreadId { get; set; }
    public DateTime Created { get; set; }

    public int DaysDifference
    {
        get
        {
            if (_daysDiffence.HasValue)
                return _daysDiffence.Value;

            _daysDiffence = (int)(DateTime.UtcNow - Created).TotalDays;
            return _daysDiffence.Value;
        }
    }
}