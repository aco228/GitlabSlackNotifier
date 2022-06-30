using GitlabSlackNotifier.Core.Domain;
using GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;
using GitlabSlackNotifier.Core.Services.Slack;
using GitlabSlackNotifier.Core.Services.Slack.Applications;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Api.Controllers;

[Route("slack-callback/[action]")]
public class SlackCallbackController : CallbackControllerBase
{
    private readonly ISlackMessagingClient _messaging;
    private readonly ISlackCommandApplicationHandler _commandHandler;
    private readonly ISlackDefaultChannel _defaultChannel;
    
    public SlackCallbackController(
        ISlackDefaultChannel slackDefaultChannel,
        ISlackCommandApplicationHandler slackCommandApplicationHandler) 
        : base("slack-callback-controller")
    {
        _commandHandler = slackCommandApplicationHandler;
        _defaultChannel = slackDefaultChannel;
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
            // Reacts on messages where @ReleaseGuy is mentioned in the message
            // Used for automatic releases
            var eventRequest = JsonConvert.DeserializeObject<EventUponMessageRequestModel>(rawRequest);
            if (eventRequest.IsValid())
            {
                await _commandHandler.RunCommand(new ()
                {
                    User = eventRequest.Event.User,
                    MessageThread = eventRequest.Event.ThreadId,
                    Channel = eventRequest.Event.Channel,
                    Text = eventRequest.Event.Text,
                });
            }
            
        }
        catch (Exception ex)
        {
            await _defaultChannel.SendMessage("Exception happened: " + Environment.NewLine + ex);
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

        }
        catch (Exception ex)
        {
            
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
