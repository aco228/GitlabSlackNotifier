using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Domain.Configuration;

public class JiraConfiguration : IConfigurationModel
{
    public string Owner { get; set; }
    public string UrlPlaceholder { get; set; }
    
    public bool AreCriticalPropertiesValid()
    {
        return !string.IsNullOrEmpty(UrlPlaceholder);
    }
}
