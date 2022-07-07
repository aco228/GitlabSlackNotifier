using Aco228.JsonToken;

namespace GitlabSlackNotifier.Core.Domain.Jira;

public class JiraIssue
{
    [JsonToken("id")]
    public int Id { get; set; }
    
    [JsonToken("fields.status.id")]
    public int Status { get; set; }
    
    [JsonToken("fields.status.name")]
    public string StatusName { get; set; }
    
    [JsonToken("fields.summary")]
    public string Title { get; set; }
}