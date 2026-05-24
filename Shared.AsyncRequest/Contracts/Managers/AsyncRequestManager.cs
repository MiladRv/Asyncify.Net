using System.Collections.Concurrent;
using System.Reflection;
using Green.CT.Asyncify.Net.Contracts.Handlers;
using Green.CT.Asyncify.Net.Contracts.Requests;
using Green.CT.Asyncify.Net.Extensions;

namespace Green.CT.Asyncify.Net.Contracts.Managers;

public class AsyncRequestManager : IAsyncRequestManager, IDisposable
{
    private readonly ConcurrentDictionary<Guid, (IAsyncRequestHandler Handler, DateTime RegisteredAt)> _requests = new();
    private readonly Timer _cleanupTimer;
    private static readonly TimeSpan ResultTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);

    public AsyncRequestManager()
    {
        _cleanupTimer = new Timer(CleanupExpiredRequests, null, CleanupInterval, CleanupInterval);
    }

    public Guid RegisterRequest(MethodInfo method, object constructor, object[] arguments)
    {
        var asyncRequestHandler = new AsyncRequestHandlerBuilder()
            .WithMethod(method)
            .WithConstructor(constructor)
            .WithArguments(arguments)
            .Build();

        _requests.TryAdd(asyncRequestHandler.Id, (asyncRequestHandler, DateTime.UtcNow));

        return asyncRequestHandler.Id;
    }

    public AsyncRequestDto Handle(Guid requestId, CancellationToken cancellationToken = default)
    {
        if (!_requests.TryGetValue(requestId, out var entry))
            throw new ArgumentNullException($"requestId {requestId} not found");

        entry.Handler.Handle(cancellationToken);

        return entry.Handler.ToDto();
    }

    public AsyncRequestDto? GetResult(Guid requestId)
    {
        return !_requests.TryGetValue(requestId, out var entry)
            ? default
            : entry.Handler.ToDto();
    }

    private void CleanupExpiredRequests(object? state)
    {
        var expiredKeys = _requests
            .Where(kvp =>
            {
                var isFinished = kvp.Value.Handler.GetStatus() is
                    AsyncRequestStatus.Complete or
                    AsyncRequestStatus.Failed or
                    AsyncRequestStatus.Timeout;
                var isExpired = DateTime.UtcNow - kvp.Value.RegisteredAt > ResultTtl;
                return isFinished && isExpired;
            })
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
            _requests.TryRemove(key, out _);
    }

    public void Dispose()
    {
        _cleanupTimer.Dispose();
    }
}