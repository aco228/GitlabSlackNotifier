using System.Net.Http.Headers;
using Aco228.SimpleHttpClient;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackHttpClient : RequestClient, ISlackHttpClient 
{
    const string BASE_URL = "https://slack.com/api/";
    
    public SlackHttpClient (ISlackConfigurationSection slackConfiguration)
    {
        var configuration = slackConfiguration.GetConfiguration();
        if(configuration == null || !configuration.AreCriticalPropertiesValid())
            throw new ArgumentException("Slack OAuth token missing from configuration");
        
        AddAuthorization(configuration.OAuth);
        SetBaseString(BASE_URL);
    }

    protected override StringContent OnAddingHeaders(StringContent content)
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));
        return content;
    }
}
