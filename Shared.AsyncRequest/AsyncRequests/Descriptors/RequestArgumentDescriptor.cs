using System.Reflection;
using Green.CT.Asyncify.Net.Contracts;
using Green.CT.Asyncify.Net.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Green.CT.Asyncify.Net.AsyncRequests.Descriptors;

internal abstract class RequestArgumentDescriptor : IRequestArgumentDescriptor
{
    protected RequestArgumentDescriptor(HttpRequest httpRequest)
    {
        Request = httpRequest;
        Arguments = new Dictionary<int, object?>();
        EndPoint = httpRequest.HttpContext.GetEndpoint() ?? throw new ArgumentNullException(nameof(EndPoint));
        FindController();
        FindMethod();

        GetRouteArguments();
        FillArguments();
    }


    protected HttpRequest Request { get; init; }
    private IDictionary<int, object?> Arguments { get; }
    private Endpoint EndPoint { get; init; }
    private Type Controller { get; set; }
    protected MethodInfo Method { get; set; }

    protected virtual void FillArguments()
    {
    }

    public object?[] GetArgument() => Arguments
        .OrderBy(a => a.Key)
        .Select(a => a.Value)
        .ToArray();

    internal void AddArgument(int position, object? value)
    {
        if (Arguments.Keys.Any(k => k.Equals(position)))
            throw new IndexOutOfRangeException();

        Arguments.Add(position, value);
    }

    private void FindController()
    {
        var attribute = EndPoint
            .Metadata
            .OfType<AsyncControllerAttribute>()
            .FirstOrDefault();

        if (attribute == null)
            throw new ArgumentNullException($"{nameof(AsyncControllerAttribute)} not found");

        Controller = attribute
            .GetControllerType()!;

    }
    private void GetRouteArguments()
    {
        var fromRouteValues = Method
            .GetParameters()
            .Where(p => p.CustomAttributes
                .Any(c => c.AttributeType == typeof(FromRouteAttribute)))
            .ToList();

        foreach (var parameterInfo in fromRouteValues)
        {
            var value = Request
                .HttpContext
                .GetRouteData()
                .Values[parameterInfo.Name];

            var convertedValue = ConverterExtensions.Convert(value, parameterInfo.ParameterType);

            Arguments
                .Add(parameterInfo.Position, convertedValue);
        }
    }
    private void FindMethod()
    {
        var attribute = EndPoint
            .Metadata
            .OfType<AsyncRequestAttribute>()
            .FirstOrDefault();

        if (attribute == null)
            throw new ArgumentNullException($"{nameof(AsyncRequestAttribute)} not found");

        Method = Controller
            .GetMethods()
            .SingleOrDefault(c => c.Name.Equals(attribute.GetMethodName()))!;
    }

    internal List<ParameterInfo> GetInputArguments(Type attribute)
    {
        return Method
            .GetParameters()
            .Where(p => p.CustomAttributes
                .Any(c => c.AttributeType == attribute))
            .ToList();
    }
}