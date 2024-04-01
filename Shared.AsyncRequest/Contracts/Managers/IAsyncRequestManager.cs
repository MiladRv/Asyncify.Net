using System.Reflection;
using Green.CT.Asyncify.Net.Contracts.Requests;

namespace Green.CT.Asyncify.Net.Contracts.Managers;

public interface IAsyncRequestManager
{
    Guid RegisterRequest(MethodInfo httpContext,
        object endpointRequestDelegate,
        object[] arguments);
    AsyncRequestDto Handle(Guid requestId,
        CancellationToken cancellationToken = default);
    AsyncRequestDto? GetResult(Guid requestId);
}