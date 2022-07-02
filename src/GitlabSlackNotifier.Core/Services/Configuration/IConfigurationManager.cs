namespace GitlabSlackNotifier.Core.Services.Configuration;

public interface IConfigurationManager
{
    T? GetConfiguration<T>(string configurationKey) where T : IConfigurationModel;
}