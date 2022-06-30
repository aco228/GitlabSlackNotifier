using GitlabSlackNotifier.Core.Applications.Slack.Commands;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Infrastructures.Slack;
using GitlabSlackNotifier.Core.Services.Configuration;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace GitlabSlackNotifier.Core.Services;

public static class ServicesExtensions
{
    public static void RegisterConfigurations(this IServiceCollection service)
    {
        service.AddTransient<IConfigurationManager, ConfigurationManager>();
        service.AddTransient<ISlackConfigurationSection, SlackConfigurationSection>();
    }
    
    public static void RegisterSlackServices(this IServiceCollection service)
    {
        service.AddSingleton<ISlackHttpClient, SlackHttpClient>();
        service.AddSingleton<ISlackCommandApplicationHandler, SlackCommandApplicationHandler>();
        
        service.AddTransient<ISlackMessagingClient, SlackMessagingClient>();
        service.AddTransient<ISlackDefaultChannel, SlackDefaultChannel>();

        service.AddTransient<IHelloSlackCommand, HelloSlackCommand>();
        service.AddTransient<ITestSlackCommand, TestSlackCommand>();
    }
}