using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Slack.Channels;

public class SlackMessageResponse
{
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("ts")]
    public string MessageThread { get; set; }
    
    [JsonProperty("user")]
    public string User { get; set; }

    public DateTime? GetDate()
    {
        var split = MessageThread.Split(".");
        if (!long.TryParse(split[0], out var epoch))
            return null;
        
        var dtDateTime = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(epoch).ToLocalTime();
        return dtDateTime;
    }

    public bool GetDate(out DateTime result)
    {
        result = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);;
        
        var split = MessageThread.Split(".");
        if (!long.TryParse(split[0], out var epoch))
            return false;
        
        result = result.AddSeconds(epoch).ToLocalTime();
        return true;
    }
    
}