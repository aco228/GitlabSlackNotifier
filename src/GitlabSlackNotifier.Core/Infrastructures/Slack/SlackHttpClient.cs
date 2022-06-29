using System.Net.Http.Headers;
using Aco228.SimpleHttpClient;
using GitlabSlackNotifier.Core.Services.Slack;
using Microsoft.Extensions.Configuration;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackHttpClient : RequestClient, ISlackHttpClient 
{
    const string BASE_URL = "https://slack.com/api/";
    
    public SlackHttpClient (IConfiguration configuration)
    {
        var oauthToken = configuration["Slack:OAuth"];
        if (string.IsNullOrEmpty(oauthToken))
            throw new ArgumentException("Slack OAuth token missing from configuration");
        
        AddAuthorization(oauthToken);
        SetBaseString(BASE_URL);
    }

    protected override StringContent OnAddingHeaders(StringContent content)
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));
        return content;
    }
}