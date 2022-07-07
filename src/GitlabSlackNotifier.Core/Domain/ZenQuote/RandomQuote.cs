using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.ZenQuote;

public class RandomQuote
{
    [JsonProperty("q")]
    public string Text { get; set; }
    
    [JsonProperty("a")]
    public string Author { get; set; }
}