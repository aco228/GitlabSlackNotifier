using GitlabSlackNotifier.Core.Services.Slack;

namespace GitlabSlackNotifier.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISlackDefaultChannel _defaultChannel;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ISlackDefaultChannel defaultChannel)
    {
        _next = next;
        _defaultChannel = defaultChannel;
    }
        
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await _defaultChannel.SendMessage("ErrorHandlingMiddleware " + ex);
        }
    }
}