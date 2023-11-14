#if UNITY_IOS
using System.Collections.Generic;
using System.Threading.Tasks;


internal class BridgeCallback
{
    public object callback;
    public BridgeCallback(object callback)
    {
        this.callback = callback;
    }
}

internal class NetflixBridgeCallbackManager
{
    private readonly object correlationIdLock = new object();
    private int correlationId = 0;
    private readonly Dictionary<int, BridgeCallback> completionSources = new Dictionary<int, BridgeCallback>();

    public static NetflixBridgeCallbackManager shared = new NetflixBridgeCallbackManager();
    private NetflixBridgeCallbackManager() { }

    public int AddCompletionSource<T>(TaskCompletionSource<T> completionSource)
    {
        if (completionSource != null)
        {
            lock (completionSources)
            {
                var correlationId = NextCorrelationId();
                completionSources[correlationId] = new BridgeCallback(completionSource);
                return correlationId;
            }
        }

        return -1;
    }

    public TaskCompletionSource<T> CompletionSourceForId<T>(int correlationId)
    {
        lock (completionSources)
        {
            var result = completionSources[correlationId];

            if (result.callback is TaskCompletionSource<T>)
            {
                return result.callback as TaskCompletionSource<T>;
            }

            return null;
        }
    }

    private int NextCorrelationId()
    {
        lock (correlationIdLock)
        {
            return ++correlationId;
        }
    }

    public void RemoveCompletionSourceForId(int correlationId)
    {
        lock (completionSources)
        {
            completionSources.Remove(correlationId);
        }
    }

    public void ResetCompletionSources()
    {
        lock (completionSources)
        {
            completionSources.Clear();
        }
    }
}
#endif