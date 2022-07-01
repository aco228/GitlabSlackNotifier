using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;

public class TextSection : BlockBase
{
    public override string Type { get; } = "section";
    
    [JsonProperty("text")]
    public TextElement Text { get; set; }

    public TextSection(string text)
    {
        Text = new TextElement(text);
    }
}