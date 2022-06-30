namespace GitlabSlackNotifier.Core.Services.Deserializers;

public interface IQueryDeserializer
{
    bool TryDeserialize<T>(string input, out T? result);
}