using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

public class HeaderElement : BlockBase
{
    public override string Type { get; } = "header";
    
    [JsonProperty("text")]
    public PlainTextElement Text { get; set; }

    public HeaderElement (string text)
    {
        Text = new PlainTextElement(text);
    }
}