using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

public class PlainTextElement : BlockBase
{
    public override string Type { get; } = "plain_text";
    
    [JsonProperty("text")]
    public string Text { get; set; }

    public PlainTextElement(string text)
    {
        Text = text;
    }
}