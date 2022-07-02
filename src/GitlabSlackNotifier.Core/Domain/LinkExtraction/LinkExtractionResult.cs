namespace GitlabSlackNotifier.Core.Domain.LinkExtraction;

public record LinkExtractionResult
{
    private int? _daysDifference = null;
    
    public string Key => $"{ProjectName}/{PullRequestId}";

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
            if (_daysDifference.HasValue)
                return _daysDifference.Value;

            _daysDifference = (int)(DateTime.UtcNow - Created).TotalDays;
            return _daysDifference.Value;
        }
    }
}