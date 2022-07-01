using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;

public class ImageSection : BlockBase
{
    public override string Type { get; } = "section";
    
    [JsonProperty("text")]
    public TextElement TextElement { get; set; }
    
    [JsonProperty("accessory")]
    public ImageElement Image { get; set; }

    public ImageSection (string text, string imageUrl, string imageAlt)
    {
        TextElement = new TextElement(text);
        Image = new ImageElement(imageUrl, imageAlt);
    }
}