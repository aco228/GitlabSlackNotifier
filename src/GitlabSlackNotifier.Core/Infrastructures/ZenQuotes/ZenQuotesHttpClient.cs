using Aco228.SimpleHttpClient;
using GitlabSlackNotifier.Core.Services.ZenQuote;

namespace GitlabSlackNotifier.Core.Infrastructures.ZenQuotes;

public class ZenQuotesHttpClient:  RequestClient, IZenQuoteHttpClient
{
    private const string BASE_URL = "https://zenquotes.io/api/";

    public ZenQuotesHttpClient ()
    {
        SetBaseString(BASE_URL);
    }
}