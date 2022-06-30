using GitlabSlackNotifier.Core.Applications.Slack.Commands;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.AspNetCore.Builder;

namespace GitlabSlackNotifier.Core;

public static class ApplicationExtensions
{
    public static void RegisterSlackCommands(this IApplicationBuilder builder)
    {
        var slackCommandHandler = builder.ApplicationServices.GetService(typeof(ISlackCommandApplicationHandler)) as ISlackCommandApplicationHandler;
        if (slackCommandHandler == null)
            throw new ArgumentException("SlackCommandHandler is null for some reason!");
        
        slackCommandHandler.AddCommand(typeof(IHelloSlackCommand));
        slackCommandHandler.AddCommand(typeof(ITestSlackCommand));
        slackCommandHandler.AddCommand(typeof(ISlackTestReadMessagesCommand));
    }
}