using GitlabSlackNotifier.Core.Infrastructures.Slack;
using GitlabSlackNotifier.Core.Services.Slack;
using Microsoft.Extensions.DependencyInjection;

namespace GitlabSlackNotifier.Core.Services;

public static class ServicesExtensions
{
    public static void RegisterSlackServices(this IServiceCollection service)
    {
        service.AddSingleton<ISlackHttpClient, SlackHttpClient>();
        service.AddTransient<ISlackMessagingClient, SlackMessagingClient>();
    }
}