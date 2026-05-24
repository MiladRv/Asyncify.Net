using System.Reflection;
using Green.CT.Asyncify.Net.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Green.CT.Asyncify.Net.Contracts.Managers;

public interface IAsyncRequestManager
{
    Guid RegisterRequest(MethodInfo method,
        object constructor,
        object[] arguments,
        IServiceScope scope);
    AsyncRequestDto Handle(Guid requestId,
        CancellationToken cancellationToken = default);
    AsyncRequestDto? GetResult(Guid requestId);
}