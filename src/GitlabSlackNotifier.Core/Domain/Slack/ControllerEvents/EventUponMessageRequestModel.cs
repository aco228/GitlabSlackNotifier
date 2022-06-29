using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;

public record EventUponMessageRequestModel
{
    [JsonProperty("type")]
    public string Type { get; set; }
        
    [JsonProperty("event_time")]
    public long EventTime { get; set; }
        
    [JsonProperty("event")]
    public EventModel Event { get; set; }
}

public record EventModel
{
    [JsonProperty("client_msg_id")]
    public string ClientMessageId { get; set; }
        
    [JsonProperty("type")]
    public string Type { get; set; }
        
    [JsonProperty("user")]
    public string User { get; set; }
        
    [JsonProperty("ts")]
    public string ThreadId { get; set; }
        
    [JsonProperty("channel")]
    public string Channel { get; set; }
        
    [JsonProperty("text")]
    public string Text { get; set; }
        
    // TODO: Temp solution
    public string Message => new Regex("\".+\"").Match(Text).Value.Replace("\"", string.Empty);
}
