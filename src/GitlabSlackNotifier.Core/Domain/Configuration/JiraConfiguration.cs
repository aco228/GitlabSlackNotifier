using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Domain.Configuration;

public class JiraConfiguration : IConfigurationModel
{
    public string Owner { get; set; }
    public string UrlPlaceholder { get; set; }
    public string UserEmail { get; set; }
    public string UserToken { get; set; }
    public string ApiBaseUrl { get; set; }
    
    public bool AreCriticalPropertiesValid()
    {
        return !string.IsNullOrEmpty(UrlPlaceholder)
               || !string.IsNullOrEmpty(UserEmail)
               || !string.IsNullOrEmpty(UserToken)
               || !string.IsNullOrEmpty(ApiBaseUrl);
    }
}
