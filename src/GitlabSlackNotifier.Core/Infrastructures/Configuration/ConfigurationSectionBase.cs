using GitlabSlackNotifier.Core.Services.Configuration;

namespace GitlabSlackNotifier.Core.Infrastructures.Configuration;

public abstract class ConfigurationSectionBase<T> : IConfigurationSection<T>
    where T : IConfigurationModel
{
    private readonly string _configurationKey;
    private readonly IConfigurationManager _configurationManager;
    
    public ConfigurationSectionBase (
        string configurationKey,
        IConfigurationManager configurationManager)
    {
        _configurationKey = configurationKey;
        _configurationManager = configurationManager;
    }

    public T? GetConfiguration()
        => _configurationManager.GetConfiguration<T>(_configurationKey);
}