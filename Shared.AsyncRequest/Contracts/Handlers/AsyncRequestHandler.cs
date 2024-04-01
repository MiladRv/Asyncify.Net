using Green.CT.Asyncify.Contracts.Requests;

namespace Green.CT.Asyncify.Contracts.Handlers;

internal sealed class AsyncRequestHandler(IAsyncRequest asyncRequest)
    : IAsyncRequestHandler
{
    public void Handle(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            asyncRequest.Invoke(cancellationToken);
        }, cancellationToken);
    }

    public AsyncRequestStatus GetStatus() => asyncRequest.GetStatus();
    public object? GetResult() => asyncRequest.GetResult();
    public Guid Id => asyncRequest.Id;
}