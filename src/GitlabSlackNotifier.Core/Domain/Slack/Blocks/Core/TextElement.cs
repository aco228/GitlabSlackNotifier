using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

public class TextElement : BlockBase
{
    public override string Type { get; } = "mrkdwn";
    
    [JsonProperty("text")]
    public string Text { get; set; }

    public TextElement(string text)
    {
        Text = text;
    }
}