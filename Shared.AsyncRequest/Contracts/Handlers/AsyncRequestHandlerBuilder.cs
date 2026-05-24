using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Green.CT.Asyncify.Net.Contracts.Handlers;

internal class AsyncRequestHandlerBuilder
{
    private MethodInfo _method;
    private object _constructor;
    private object[] _arguments;
    private IServiceScope _scope;

    public AsyncRequestHandlerBuilder WithMethod(MethodInfo method)
    {
        _method = method;
        return this;
    }

    public AsyncRequestHandlerBuilder WithConstructor(object constructor)
    {
        _constructor = constructor;
        return this;
    }

    public AsyncRequestHandlerBuilder WithArguments(object[] arguments)
    {
        _arguments = arguments;
        return this;
    }

    public AsyncRequestHandlerBuilder WithScope(IServiceScope scope)
    {
        _scope = scope;
        return this;
    }

    public IAsyncRequestHandler Build()
    {
        if (_method is null)
            throw new InvalidOperationException($"{nameof(WithMethod)} must be called before Build().");

        if (_constructor is null)
            throw new InvalidOperationException($"{nameof(WithConstructor)} must be called before Build().");

        if (_arguments is null)
            throw new InvalidOperationException($"{nameof(WithArguments)} must be called before Build().");

        if (_scope is null)
            throw new InvalidOperationException($"{nameof(WithScope)} must be called before Build().");

        var asyncRequest = new Requests.AsyncRequest(_method,
            _constructor,
            _arguments,
            _scope);

        var handler = new AsyncRequestHandler(asyncRequest);

        return handler;
    }
}