using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Parts;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Elements;

public class Checkbox : ActionElement<CheckboxElement>
{
    public Checkbox(string actionName)
    {
        Elements.Add(new CheckboxElement
        {
            ActionId = actionName
        });
    }

    public void Add(CheckboxOption checkbox)
        => Elements.FirstOrDefault().Options.Add(checkbox);
}