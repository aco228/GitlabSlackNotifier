using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Api.Controllers;

[Route("slack-callback/[action]")]
public class SlackCallbackController : CallbackControllerBase
{
    public SlackCallbackController() 
        : base("slack-callback-controller")
    {
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
