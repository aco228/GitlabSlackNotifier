using System.Net;
using System.Text;
using System.Web;
using GitlabSlackNotifier.Api.Controllers.Webhooks;
using GitlabSlackNotifier.Core;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace GitlabSlackNotifier.Api.Controllers;

public abstract class CallbackControllerBase : ControllerBase
{
    private readonly string _webhookIdentifier;
    private static Dictionary<string, WebhookControllerModel> _models = new();

    protected CallbackControllerBase (string webhookIdentifier)
    {
        _webhookIdentifier = webhookIdentifier;
        if (!_models.ContainsKey(webhookIdentifier))
            _models.Add(webhookIdentifier, new WebhookControllerModel());
    }
    
    private WebhookControllerModel Model 
        => _models.GetValueOrDefault(_webhookIdentifier)!;
    
    protected virtual string ProcessBodyText(string bodyText) => bodyText;
    
    [HttpGet]
    public IActionResult Messages()
    {
        // if (User?.Identity?.IsAuthenticated == false)
        //     return Content("No access");

        var htmlBuilder = new StringBuilder();
        var br = "<br>";
        
        foreach (var message in Model.Messages)
        {
            var messageBlock = $"{message.Date.Humanize()} - {message.HttpMethod} - {message.Endpoint}" + br
                + $"<span>{message.RawResponse}</span>" + br
                + br + br + br + br;
            htmlBuilder.Append(messageBlock);
        }

        return new ContentResult {
            ContentType = "text/html",
            StatusCode = (int)HttpStatusCode.OK,
            Content = htmlBuilder.ToString()
        };
    }
    
    protected async Task<string> ReadBody()
    {
        var bodyStream = new StreamReader(Request.Body);
        var bodyText = await bodyStream.ReadToEndAsync();
        bodyText = ProcessBodyText(HttpUtility.UrlDecode(bodyText));
            
        if (string.IsNullOrEmpty(bodyText))
            return null!;
            
        var sha = bodyText.ToSha256();
        
        // return nothing if we have cached this message previously
        if (Model.Contains(sha))
            return null!;

        var callbackMessage = new CallbackMessage
        {
            Sha = sha,
            Date = DateTime.Now,
            Endpoint = Request.Path,
            HttpMethod = Request.Method,
            RawResponse = bodyText,
        };
            
        Model.Add(callbackMessage);
        return bodyText;
    }
    
}
