using GitlabSlackNotifier.Core.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Infrastructures.Configuration;

public class ConfigurationManager : IConfigurationManager
{
    private readonly IConfiguration _configuration;
    
    public ConfigurationManager (IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T? GetConfiguration<T>(string configurationKey)
        where T : IConfigurationModel
    {
        var configurationEnvironmentKey = _configuration[$"Environment:{configurationKey}"];

        if (string.IsNullOrEmpty(configurationEnvironmentKey))
            return default;
        
        if (!string.IsNullOrEmpty(configurationKey) 
            && EnvironmentConfigurationExists(configurationEnvironmentKey, out T? configuration))
            return configuration!;

        return _configuration.GetSection(configurationKey).Get<T>();
    }

    private bool EnvironmentConfigurationExists<T>(string key, out T? configuration)
        where T : IConfigurationModel
    {
        configuration = default;

        var json = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(json))
            return false;

        configuration = JsonConvert.DeserializeObject<T>(json);
        return configuration != null;
    }
}