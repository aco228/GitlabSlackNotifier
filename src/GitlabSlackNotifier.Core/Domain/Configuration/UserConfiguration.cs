using GitlabSlackNotifier.Core.Domain.Persistency;
using GitlabSlackNotifier.Core.Services.Configuration;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Domain.Configuration;

public class UsersConfiguration : IConfigurationModel
{
    [JsonProperty("Db")]
    public List<User> Db { get; set; }

    public bool AreCriticalPropertiesValid()
    {
        return true;
    }
}