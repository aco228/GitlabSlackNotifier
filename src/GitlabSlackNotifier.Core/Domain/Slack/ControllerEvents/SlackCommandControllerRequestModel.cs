using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;

public class SlackCommandControllerRequestModel
{
    [JsonProperty("token")]
    public string Token { get; set; }
    
    [JsonProperty("team_id")]
    public string TeamId { get; set; }
    
    [JsonProperty("channel_id")]
    public string ChannelId { get; set; }
    
    [JsonProperty("channel_name")]
    public string ChannelName { get; set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; set; }
    
    [JsonProperty("user_name")]
    public string Username { get; set; }
    
    [JsonProperty("command")]
    public string Command { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("api_app_id")]
    public string ApiAppId { get; set; }
    
    [JsonProperty("response_url")]
    public string ResponseUrl { get; set; }
    
    [JsonProperty("trigger_id")]
    public string TriggerId { get; set; }
}