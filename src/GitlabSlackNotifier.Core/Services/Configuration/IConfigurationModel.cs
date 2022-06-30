namespace GitlabSlackNotifier.Core.Services.Configuration;

public interface IConfigurationModel
{
    bool AreCriticalPropertiesValid();
}