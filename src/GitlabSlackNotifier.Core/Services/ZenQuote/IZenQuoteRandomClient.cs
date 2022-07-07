using GitlabSlackNotifier.Core.Domain.ZenQuote;

namespace GitlabSlackNotifier.Core.Services.ZenQuote;

public interface IZenQuoteRandomClient
{
    Task<RandomQuote?> GetRandomQuote();
}