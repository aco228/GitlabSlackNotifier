using GitlabSlackNotifier.Core.Domain.ZenQuote;
using GitlabSlackNotifier.Core.Services.ZenQuote;

namespace GitlabSlackNotifier.Core.Infrastructures.ZenQuotes;

public class ZenQuoteRandomClient : IZenQuoteRandomClient
{
    private readonly IZenQuoteHttpClient _client;

    public ZenQuoteRandomClient(IZenQuoteHttpClient client)
    {
        _client = client;
    }

    public async Task<RandomQuote?> GetRandomQuote()
    {
        var result = await _client.Get<List<RandomQuote>>("random");
        return result.FirstOrDefault() ?? null;
    }
}