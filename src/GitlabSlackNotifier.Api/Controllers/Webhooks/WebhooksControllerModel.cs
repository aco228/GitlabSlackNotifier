namespace GitlabSlackNotifier.Api.Controllers.Webhooks;

public class WebhookControllerModel
{
    private static readonly int MAX_MESSAGE_CACHE_COUNT = 6;
        
    private List<CallbackMessage> _messages = new();
    private object LockObj { get; } = new();

    public List<CallbackMessage> Messages
    {
        get
        {
            lock (LockObj)
            {
                return _messages.ToList();
            }
        }
    }

    public bool Contains(string sha)
    {
        lock (LockObj)
        {
            return _messages.Any(x => x.Sha.Equals(sha));
        }
    }

    public void Add(CallbackMessage message)
    {
        lock (LockObj)
        {
            _messages.Add(message);
            ClearOverflowMessages();   
        }
    }

    private void ClearOverflowMessages()
    {
        if (_messages.Count > MAX_MESSAGE_CACHE_COUNT)
            for(var i = _messages.Count - 1; i > MAX_MESSAGE_CACHE_COUNT; i--)
                _messages.RemoveAt(i);
    }
}