namespace GitlabSlackNotifier.Core.Domain.LinkExtraction;

public record LinkExtractionResult
{
    public string Name { get; set; }
    public int Value { get; set; }
    public string RawValue { get; set; }
}