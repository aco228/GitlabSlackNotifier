using GitlabSlackNotifier.Core.Applications.Slack.Commands;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Infrastructures.Configuration;
using GitlabSlackNotifier.Core.Infrastructures.Deserializers;
using GitlabSlackNotifier.Core.Infrastructures.Gitlab;
using GitlabSlackNotifier.Core.Infrastructures.Jira;
using GitlabSlackNotifier.Core.Infrastructures.Persistency;
using GitlabSlackNotifier.Core.Infrastructures.Slack;
using GitlabSlackNotifier.Core.Infrastructures.Utilities.Gitlab;
using GitlabSlackNotifier.Core.Infrastructures.Utilities.Slack;
using GitlabSlackNotifier.Core.Infrastructures.ZenQuotes;
using GitlabSlackNotifier.Core.Services.Configuration;
using GitlabSlackNotifier.Core.Services.Deserializers;
using GitlabSlackNotifier.Core.Services.Gitlab;
using GitlabSlackNotifier.Core.Services.Jira;
using GitlabSlackNotifier.Core.Services.LinkExtraction;
using GitlabSlackNotifier.Core.Services.Persistency;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using GitlabSlackNotifier.Core.Services.Utilities.Gitlab;
using GitlabSlackNotifier.Core.Services.Utilities.Slack;
using GitlabSlackNotifier.Core.Services.ZenQuote;
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
        service.AddTransient<IJiraConfigurationSection, JiraConfigurationSection>();
        service.AddTransient<IUsersConfigurationSection, UsersConfigurationSection>();
    }

    public static void RegisterGitlabServices(this IServiceCollection service)
    {
        service.AddSingleton<IGitlabHttpClient, GitlabHttpClient>();
        service.AddTransient<IGitlabAboutMeService, GitlabAboutMeService>();
        service.AddTransient<IGitlabProjectsClient, GitlabProjectsClient>();
        service.AddSingleton<IGitlabProjectsCache, GitlabProjectsCache>();
        service.AddTransient<IGitlabMergeRequestClient, GitlabMergeRequestClient>();
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

    public static void RegisterJiraServices(this IServiceCollection service)
    {
        service.AddSingleton<IJiraHttpClient, JiraHttpClient>();

        service.AddTransient<IJiraIssueClient, JiraIssueClient>();
    }

    public static void RegisterUtilities(this IServiceCollection service)
    {
        service.AddTransient<IGitlabSlackLinkExtractorUtility, GitlabSlackLinkExtractorUtility>();
        service.AddTransient<IGetSlackMessageLinkUtility, GetSlackMessageLinksUtility>();
        service.AddTransient<IConstructReportMessageUtility, ConstructReportMessageUtility>();
        service.AddTransient<IGetApprovalRulesUtility, GetApprovalRulesUtility>();
    }

    public static void RegisterPersistency(this IServiceCollection service)
    {
        service.AddSingleton<IUserRepository, ConfigurationUserRepository>();
    }

    public static void RegisterZenQuote(this IServiceCollection service)
    {
        service.AddTransient<IZenQuoteHttpClient, ZenQuotesHttpClient>();
        service.AddTransient<IZenQuoteRandomClient, ZenQuoteRandomClient>();
    }
    
}