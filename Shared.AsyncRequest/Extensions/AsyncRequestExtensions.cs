using Green.CT.Asyncify.Net.Contracts.Handlers;
using Green.CT.Asyncify.Net.Contracts.Requests;

namespace Green.CT.Asyncify.Net.Extensions;

internal static class AsyncRequestExtensions
{
    public static AsyncRequestDto ToDto(this IAsyncRequestHandler asyncRequest)
    {
        return new AsyncRequestDto(trackId: asyncRequest.Id,
            status: asyncRequest.GetStatus(),
            result: asyncRequest.GetResult(),
            createdAt: asyncRequest.CreationDate);
    }
}