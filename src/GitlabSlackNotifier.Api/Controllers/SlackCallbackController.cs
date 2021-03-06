using GitlabSlackNotifier.Core.Domain;
using GitlabSlackNotifier.Core.Domain.Slack.Application;
using GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;
using GitlabSlackNotifier.Core.Services.Deserializers;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Api.Controllers;

[Route("slack-callback/[action]")]
public class SlackCallbackController : CallbackControllerBase
{
    private readonly ILogger<SlackCallbackController> _logger;
    private readonly ISlackCommandApplicationHandler _commandHandler;
    private readonly ISlackDefaultChannel _defaultChannel;
    private readonly IQueryDeserializer _queryDeserializer;
    
    public SlackCallbackController(
        ILogger<SlackCallbackController> logger,
        ISlackDefaultChannel slackDefaultChannel,
        ISlackCommandApplicationHandler slackCommandApplicationHandler,
        IQueryDeserializer queryDeserializer) 
        : base("slack-callback-controller")
    {
        _logger = logger;
        _commandHandler = slackCommandApplicationHandler;
        _defaultChannel = slackDefaultChannel;
        _queryDeserializer = queryDeserializer;
    }

    public IActionResult Index() 
        => Content("OK"); 

    [HttpPost]
    public async Task<IActionResult> ActionEndpoint()
    {
        var rawRequest = await ReadBody();
        if (string.IsNullOrEmpty(rawRequest))
            return Ok();

        if (rawRequest.Contains("challenge"))
            return RespondToChallenge(rawRequest);
        
        try
        {
            // Reacts on messages where @GitlabSlackNotifier is mentioned in the message
            // Used for automatic releases
            var eventRequest = JsonConvert.DeserializeObject<EventUponMessageRequestModel>(rawRequest);
            if (eventRequest.IsValid())
            {
                Task.Run(() => _commandHandler.RunCommand(new ()
                {
                    User = eventRequest.Event.User,
                    MessageThread = eventRequest.Event.ThreadId,
                    Channel = eventRequest.Event.Channel,
                    Text = eventRequest.Event.Text,
                }, SlackCommandType.Mention).ConfigureAwait(false));
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception on processing slack-callback event");
            
            await _defaultChannel.SendMessage("Exception happened on slack-callback event: " + Environment.NewLine + ex);
        }

        return Ok();
    }

    public async Task<IActionResult> Command()
    {
        var rawRequest = await ReadBody();
        if (string.IsNullOrEmpty(rawRequest))
            return Ok();

        if (rawRequest.Contains("challenge"))
            return RespondToChallenge(rawRequest);

        try
        {
            if (!_queryDeserializer.TryDeserialize<SlackCommandControllerRequestModel>(rawRequest, out var eventRequest))
            {
                await _defaultChannel.SendMessage($"Could not deserialize command with this raw data: {rawRequest}");
            }

            Task.Run(() => _commandHandler.RunCommand(new ()
            {
                User = eventRequest.UserId,
                MessageThread = string.Empty,
                Channel = eventRequest.ChannelId,
                Text = $"{eventRequest.Command.Substring(1)} {eventRequest.Text}",
            }, SlackCommandType.Command).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception on processing slack-callback callback");
            
            await _defaultChannel.SendMessage("Exception happened on slack-callback command: " + Environment.NewLine + ex);
        }

        return Ok();
    }
    
    public ActionResult RespondToChallenge(string bodyText)
    {
        try
        {
            var json = JsonConvert.DeserializeObject<dynamic>(bodyText);
            var challenge = "";
            try
            {
                challenge = json.body.challenge;
            }
            catch
            {
                challenge = json.challenge;
            }
            return Content(challenge);
        }
        catch
        {
            return NoContent();
        }
    }
}
