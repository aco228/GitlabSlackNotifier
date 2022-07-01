using GitlabSlackNotifier.Core.Applications.Slack.Commands;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Infrastructures.Deserializers;
using GitlabSlackNotifier.Core.Infrastructures.Gitlab;
using GitlabSlackNotifier.Core.Infrastructures.Slack;
using GitlabSlackNotifier.Core.Services.Configuration;
using GitlabSlackNotifier.Core.Services.Deserializers;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.LinkExtraction;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace GitlabSlackNotifier.Core;

public static class ServicesExtensions
{
    public static void RegisterDeserializers(this IServiceCollection service)
    {
        service.AddTransient<IQueryDeserializer, QueryDeserializer>();
    }
    
    public static void RegisterConfigurations(this IServiceCollection service)
    {
        service.AddTransient<IConfigurationManager, ConfigurationManager>();
        service.AddTransient<ISlackConfigurationSection, SlackConfigurationSection>();
        service.AddTransient<IGitlabConfigurationSection, GitlabConfigurationSection>();
    }

    public static void RegisterGitlabServices(this IServiceCollection service)
    {
        service.AddSingleton<IGitlabHttpClient, GitlabHttpClient>();
        service.AddTransient<IGitlabAboutMeService, GitlabAboutMeService>();
        service.AddTransient<IGitlabProjectsClient, GitlabProjectsClient>();
        service.AddSingleton<IGitlabProjectsCache, GitlabProjectsCache>();
        service.AddSingleton<IGitlabSlackLinkExtractor, GitlabSlackLinkExtractor>();
    }

    public static void RegisterSlackServices(this IServiceCollection service)
    {
        service.AddSingleton<ISlackHttpClient, SlackHttpClient>();
        service.AddSingleton<ISlackCommandApplicationHandler, SlackCommandApplicationHandler>();
        service.AddSingleton<ISlackUserCache, SlackUserCache>();
        
        service.AddTransient<ISlackMessagingClient, SlackMessagingClient>();
        service.AddTransient<ISlackConversationClient, SlackConversationClient>();
        service.AddTransient<ISlackDefaultChannel, SlackDefaultChannel>();
        service.AddTransient<ISlackUserClient, SlackUserClient>();

        service.AddTransient<IHelloSlackCommand, HelloSlackCommand>();
        service.AddTransient<ITestSlackCommand, TestSlackCommand>();
        service.AddTransient<ISlackTestReadMessagesCommand, SlackTestReadMessagesCommand>();
        service.AddTransient<IReportPullRequestsCommand, ReportPullRequestsCommand>();
        
    }
    
}