using System.Reflection;
using Green.CT.Asyncify.Net.Contracts;
using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.Net.Extensions;

public static class HttpContextExtensions
{
    public static Type GetAsyncControllerType(this HttpContext httpContext)
    {
        var endPoint = httpContext.GetEndpoint() ?? throw new ArgumentNullException("EndPoint");

        var attribute = endPoint
            .Metadata
            .OfType<AsyncControllerAttribute>()
            .FirstOrDefault();

        if (attribute == null)
            throw new ArgumentNullException($"{nameof(AsyncControllerAttribute)} not found");

        return attribute.GetType()!;
    }

    public static MethodInfo GetAsyncMethodInfo(this HttpContext httpContext, Type asyncController)
    {
        var endPoint = httpContext.GetEndpoint() ?? throw new ArgumentNullException("EndPoint");


        var attribute = endPoint
            .Metadata
            .OfType<AsyncRequestAttribute>()
            .FirstOrDefault();

        if (attribute == null)
            throw new ArgumentNullException($"{nameof(AsyncRequestAttribute)} not found");

        return asyncController
            .GetMethods()
            .SingleOrDefault(c => c.Name.Equals(attribute.GetMethodName()))!;
    }

}