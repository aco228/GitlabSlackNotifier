using GitlabSlackNotifier.Core.Services.Deserializers;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Infrastructures.Deserializers;

public class QueryDeserializer : IQueryDeserializer
{
    public bool TryDeserialize<T>(string input, out T? result)
    {
        
        result = default;
        
        var split = input.Trim().Split("&");
        if (split.Length == 0)
            return false;

        result = Activator.CreateInstance<T>();
        
        var properties = result?.GetType().GetPropertyWithAttribute<JsonPropertyAttribute>() ?? null;
        if (properties == null || properties.Count == 0)
            return false;
        
        foreach (var parameter in split)
        {
            var paramValue = parameter.Split("=");
            if (paramValue.Length != 2)
                return false;

            var prop = properties
                .FirstOrDefault(x => x.Attribute?.PropertyName?.Equals(paramValue[0]) ?? false);
            
            if (prop.Attribute == null)
                continue;

            try
            {
                var propObj = Convert.ChangeType(paramValue[1], prop.Info.PropertyType);
                prop.Info.SetValue(result, propObj);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        return true;

    }
}