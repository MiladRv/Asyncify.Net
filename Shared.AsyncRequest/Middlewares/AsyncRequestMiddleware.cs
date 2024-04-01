using Green.CT.Asyncify.Net.Contracts.Managers;
using Green.CT.Asyncify.Net.Extensions;
using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.Net.Middlewares;

public class AsyncRequestMiddleware
{
    private readonly IAsyncRequestManager _asyncRequestManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly CancellationToken _cancellationToken = default;

    public AsyncRequestMiddleware(RequestDelegate next,
        IAsyncRequestManager asyncRequestHandler,
        IServiceProvider serviceProvider)
    {
        _asyncRequestManager = asyncRequestHandler;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var arguments = context.Request.GetArguments();

        var controller = context.GetAsyncControllerType();

        var method = context.GetAsyncMethodInfo(controller);

        var constructor = controller.BuildConstructor(_serviceProvider);

        var requestId = _asyncRequestManager.RegisterRequest(method,
                constructor,
                arguments);

        var resultDto = _asyncRequestManager.Handle(requestId, _cancellationToken);

        await context.Response.WriteAsJsonAsync(resultDto, _cancellationToken);
    }
}