namespace GitlabSlackNotifier.Api.Controllers.Webhooks;

public class CallbackMessage
{
    public string Sha { get; set; }
    public string RawResponse { get; set; }
    public string HttpMethod { get; set; }
    public string Endpoint { get; set; }
    public DateTime Date { get; set; }
}