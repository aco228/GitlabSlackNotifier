namespace GitlabSlackNotifier.Core.Tests.Mocks;

public class CollectResultMockService
{
    public List<object> Objects { get; private set; } = new ();

    public void Collect(params object[] args)
    {
        Objects = args.ToList();
    }
}