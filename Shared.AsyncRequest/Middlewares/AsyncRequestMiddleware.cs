using Green.CT.Asyncify.Net.Contracts.Managers;
using Green.CT.Asyncify.Net.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Green.CT.Asyncify.Net.Middlewares;

public class AsyncRequestMiddleware
{
    private readonly IAsyncRequestManager _asyncRequestManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AsyncRequestMiddleware(RequestDelegate next,
        IAsyncRequestManager asyncRequestHandler,
        IServiceScopeFactory serviceScopeFactory)
    {
        _asyncRequestManager = asyncRequestHandler;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        // یه scope جدید برای هر request میسازیم تا scoped services
        // مثل DbContext ایزوله باشن و داخل singleton گیر نکنن.
        // خود scope داخل AsyncRequest بعد از اتمام task dispose میشه.
        var scope = _serviceScopeFactory.CreateScope();

        var arguments = context.Request.GetArguments();
        var controller = context.GetAsyncControllerType();
        var method = context.GetAsyncMethodInfo(controller);
        var constructor = controller.BuildConstructor(scope.ServiceProvider);

        var requestId = _asyncRequestManager.RegisterRequest(method,
            constructor,
            arguments,
            scope);

        var resultDto = _asyncRequestManager.Handle(requestId, context.RequestAborted);

        await context.Response.WriteAsJsonAsync(resultDto, context.RequestAborted);
    }
}