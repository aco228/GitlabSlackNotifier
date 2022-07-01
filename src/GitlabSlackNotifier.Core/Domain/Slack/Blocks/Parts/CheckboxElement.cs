using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks.Parts;

public class CheckboxElement : BlockBase
{
    public override string Type { get; } = "checkboxes";
        
    public List<CheckboxOption> Options { get; set; } = new();
        
    [JsonProperty("action_id")]
    public string ActionId { get; set; }
        
}