using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

public class ImageElement : BlockBase
{
    public override string Type { get; } = "image";
    
    [JsonProperty("image_url")]
    public string Url { get; set; }
    
    [JsonProperty("alt_text")]
    public string AltText { get; set; }

    public ImageElement (string url, string alt)
    {
        Url = url;
        AltText = alt;
    }
}