using Green.CT.Asyncify.Net.Contracts.Requests;

namespace Green.CT.Asyncify.Net.Contracts.Handlers;

public interface IAsyncRequestHandler
{
    void Handle(CancellationToken cancellationToken);
    AsyncRequestStatus GetStatus();
    object? GetResult();
    Guid Id { get; }
}