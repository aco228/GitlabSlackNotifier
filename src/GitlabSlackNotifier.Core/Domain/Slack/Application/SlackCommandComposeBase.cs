using System.Text;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Domain.Slack.Application;

public abstract class SlackCommandComposeBase<T> : SlackCommandBase, ISlackApplicationCommand
    where T : class, new()
{
    protected SlackCommandComposeBase(IServiceProvider serviceProvider) 
        : base(serviceProvider) { }
    
    public abstract string Description { get; }
    
    private class ArgumentPropertyInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    
    protected abstract Task Process(SlackCommandRequest request, T model);

    public override Task Process(SlackCommandRequest request, string[] inputArguments)
    {
        try
        {
            if (inputArguments.First().ToLower().Equals("help"))
                return PrintHelp(request);
            
            if (!GetArgumentProperties(inputArguments, out var arguments))
                return ReportBackMessage(request, "Could not parse arguments");

            var model = Activator.CreateInstance<T>();
            var typeProperties = model.GetType().GetPropertyWithAttribute<CommandPropertyAttribute>() ?? null;
            if (typeProperties == null || typeProperties.Count == 0)
                return ReportBackMessage(request, "Error in command model");

            foreach (var property in typeProperties)
            {
                if (property.Attribute == null)
                    return ReportBackMessage(request, $"Error in command model (prop={property.Info.Name} has no attribute)");
                
                var propValue = arguments.FirstOrDefault(x => x.Name.Equals(property.Attribute.Name));
                if (propValue == null)
                {
                    if (property.Attribute.Required)
                        return ReportBackMessage(request, $"Property {property.Attribute.Name} is required to be set");

                    continue;
                }
                
                try
                {
                    var typeToConvert = Nullable.GetUnderlyingType(property.Info.PropertyType) 
                                        ?? property.Info.PropertyType;
                    var propObj = Convert.ChangeType(propValue.Value, typeToConvert);
                    property.Info.SetValue(model, propObj);
                }
                catch(Exception ex)
                {
                    var logger = ServiceProvider.GetService(typeof(ILogger<SlackCommandComposeBase<T>>)) as ILogger<SlackCommandComposeBase<T>>;
                    logger.LogCritical(ex, $"Could not parse {property.Attribute.Name}={propValue.Value} to type {property.Info.PropertyType.Name}");
                    
                    return ReportBackMessage(request, $"Could not parse {property.Attribute.Name}={propValue.Value} to type {property.Info.PropertyType.Name}");
                }
            }

            return Process(request, model);
        }
        catch (Exception ex)
        {
            var logger = ServiceProvider.GetService(typeof(ILogger<SlackCommandComposeBase<T>>)) as ILogger<SlackCommandComposeBase<T>>;
            logger.LogCritical(ex, "Exception on processing slack command base");
            
            return ReportBackMessage(request, "Exception parsing arguments");
        }
    }

    public async Task PrintHelp(SlackCommandRequest request)
    {
        await ReportBackMessage(request, $"Help for command `{CommandName}`");
        await ReportBackMessage(request, Description);
        
        var model = Activator.CreateInstance<T>();
        var typeProperties = model.GetType().GetPropertyWithAttribute<CommandPropertyAttribute>() ?? null;

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Properties for this command" + Environment.NewLine + Environment.NewLine);
        
        foreach (var prop in typeProperties)
        {
            stringBuilder.Append($"`{prop.Attribute.Name}` ");
            if(prop.Attribute.Required)
                stringBuilder.Append($"*REQUIRED* ");

            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append(prop.Attribute.Description);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append(Environment.NewLine);
        }

        await ReportBackMessage(request, stringBuilder.ToString());
    }


    /// <summary>
    /// As we are receiving arguments, we are expecting that argument be in format of
    /// ar1=val1 arg2=val2 ...
    /// So with that assumption we are parsing them
    /// </summary>
    private bool GetArgumentProperties(string[] arguments, out List<ArgumentPropertyInfo> result)
    {
        result = new();
        
        foreach (var arg in arguments)
        {
            if(string.IsNullOrEmpty(arg))
                continue;
            
            var split = arg.Split("=");
            
            if (split.Length != 2)
                return false;
            
            result.Add(new ArgumentPropertyInfo
            {
                Name = split[0].Trim(),
                Value = split[1].Trim()
            });
        }
        return true;
    }
    
    
}
