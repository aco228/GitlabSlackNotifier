namespace GitlabSlackNotifier.Core.Services.Configuration;

public interface IConfigurationSection<T>
{
    T GetConfiguration();
}