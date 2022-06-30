using System.Text.RegularExpressions;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;

namespace GitlabSlackNotifier.Core.Infrastructures.Slack;

public class SlackCommandApplicationHandler : ISlackCommandApplicationHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISlackMessagingClient _messagingClient;
    private HashSet<Type> _commands = new();

    public SlackCommandApplicationHandler (
        IServiceProvider serviceProvider,
        ISlackMessagingClient messagingClient)
    {
        _serviceProvider = serviceProvider;
        _messagingClient = messagingClient;
    }
    
    public Task RunCommand(SlackCommandRequest request)
    {
        var mention = new Regex("<@.+>").Match(request.Text)?.Value ?? string.Empty;
        var requestText = request.Text.Replace(mention, string.Empty).Trim();

        var split = requestText.Split(" ").ToList();
        if (split.Count == 0)
            return RespondWithMessage(request, "No command!!");
        
        var commandName = split.FirstOrDefault();
        if (string.IsNullOrEmpty(commandName))
            return RespondWithMessage(request, "Missing command name");
        
        split.RemoveAt(0);
        if (!GetCommand(commandName, out var command))
            return RespondWithMessage(request, $"There is no command with name `{commandName}`. Call `help` command for more details");

        return command.Process(request, split.ToArray());
    }

    private Task RespondWithMessage(SlackCommandRequest request, string message)
        => _messagingClient.PublishMessage(new ()
        {
            Thread = request.MessageThread,
            ChannelId = request.Channel,
            Message = message
        });

    public bool GetCommand(string commandName, out ISlackApplicationCommand command)
    {
        command = null;
        foreach (var serviceType in _commands)
        {
            var service = _serviceProvider.GetService(serviceType) as ISlackApplicationCommand;
            if (service != null && service.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                command = service;
                return true;
            }
        }

        return false;
    }

    public void AddCommand(Type command)
        => _commands.Add(command);
}