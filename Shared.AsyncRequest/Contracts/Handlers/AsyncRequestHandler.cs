using Green.CT.Asyncify.Net.Contracts.Requests;

namespace Green.CT.Asyncify.Net.Contracts.Handlers;

internal sealed class AsyncRequestHandler(IAsyncRequest asyncRequest)
    : IAsyncRequestHandler
{
    public void Handle(CancellationToken cancellationToken)
    {
        asyncRequest.Invoke(cancellationToken);
    }

    public AsyncRequestStatus GetStatus() => asyncRequest.GetStatus();
    public object? GetResult() => asyncRequest.GetResult();
    public Guid Id => asyncRequest.Id;
    public DateTime CreationDate => asyncRequest.CreationDate;
}