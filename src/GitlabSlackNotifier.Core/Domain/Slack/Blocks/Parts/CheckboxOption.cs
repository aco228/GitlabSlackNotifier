using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Parts;

public class CheckboxOption
{
    public TextElement Text { get; set; }
    public TextElement Description { get; set; }
    public string Value { get; set; }

    public CheckboxOption(string text, string identifier, string description = "")
    {
        Text = new TextElement(text);
        Value = identifier;
            
        if (!string.IsNullOrEmpty(description))
            Description = new TextElement(description);
    }
}