using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Domain.Configuration;

public class SlackConfiguration : IConfigurationModel
{
    public string SlackOwner { get; set; }
    public string AppId {get;set;}
    public string ClientId {get;set;}
    public string ClientSecret {get;set;}
    public string SigningSecret {get;set;}
    public string MainChannelId {get;set;}
    public string OAuth {get;set;}

    public bool AreCriticalPropertiesValid()
        => !string.IsNullOrEmpty(ClientId)
           && !string.IsNullOrEmpty(ClientSecret)
           && !string.IsNullOrEmpty(MainChannelId)
           && !string.IsNullOrEmpty(OAuth)
           && !string.IsNullOrEmpty(SlackOwner);
}