using System.Collections.Concurrent;
using System.Reflection;
using Green.CT.Asyncify.Net.Contracts.Handlers;
using Green.CT.Asyncify.Net.Contracts.Requests;
using Green.CT.Asyncify.Net.Extensions;

namespace Green.CT.Asyncify.Net.Contracts.Managers;

public class AsyncRequestManager : IAsyncRequestManager
{
    private readonly ConcurrentDictionary<Guid, IAsyncRequestHandler> _requests = new();

    public Guid RegisterRequest(MethodInfo method, object constructor, object[] arguments)
    {
        var asyncRequestHandler = new AsyncRequestHandlerBuilder()
            .WithMethod(method)
            .WithConstructor(constructor)
            .WithArguments(arguments)
            .Build();
        
        _requests.TryAdd(asyncRequestHandler.Id, asyncRequestHandler);

        return asyncRequestHandler.Id;
    }

    public AsyncRequestDto Handle(Guid requestId, CancellationToken cancellationToken = default)
    {
        if (!_requests.TryGetValue(requestId, out var asyncRequestHandler))
            throw new ArgumentNullException($"requestId {requestId} not found");

        asyncRequestHandler.Handle(cancellationToken);

        return asyncRequestHandler.ToDto();
    }

    public AsyncRequestDto? GetResult(Guid requestId)
    {
        return !_requests.TryGetValue(requestId, out var asyncRequestHandler) 
            ? default 
            : asyncRequestHandler.ToDto();
    }
}