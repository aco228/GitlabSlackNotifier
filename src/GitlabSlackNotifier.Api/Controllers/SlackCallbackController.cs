using GitlabSlackNotifier.Core.Domain;
using GitlabSlackNotifier.Core.Domain.Slack.ControllerEvents;
using GitlabSlackNotifier.Core.Services.Slack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Api.Controllers;

[Route("slack-callback/[action]")]
public class SlackCallbackController : CallbackControllerBase
{
    private readonly ISlackMessagingClient _messaging;
    private readonly string _slackMainChannelId;
    
    public SlackCallbackController(
        IConfiguration configuration,
        ISlackMessagingClient messagingClient) 
        : base("slack-callback-controller")
    {
        _slackMainChannelId = configuration["Slack:MainChannelId"];
        _messaging = messagingClient;
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
                await _messaging.PublishMessage(new ()
                {
                    ChannelId = eventRequest.Event.Channel,
                    Thread = eventRequest.Event.ThreadId,
                    Message = "Hey there :)"
                });
            }
            
        }
        catch (Exception ex)
        {
            await _messaging.PublishMessage(new ()
            {
                ChannelId = _slackMainChannelId,
                Message = "Exception happened: " + Environment.NewLine + ex
            });
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
