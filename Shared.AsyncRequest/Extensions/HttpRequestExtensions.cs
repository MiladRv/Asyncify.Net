using Green.CT.Asyncify.Net.AsyncRequests.Descriptors;
using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.Net.Extensions;

public static class HttpRequestExtensions
{
    public static object[] GetArguments(this HttpRequest httpRequest)
    {
        RequestArgumentDescriptor descriptor;
        var httpMethod = httpRequest.Method.CastToHttpMethod();

        if (httpMethod == HttpMethod.Get)
            descriptor = new GetRequestArgumentDescriptor(httpRequest);
        else if (httpMethod == HttpMethod.Post)
            descriptor = new PostRequestArgumentDescriptor(httpRequest);
        else if (httpMethod == HttpMethod.Put)
            descriptor = new PutRequestArgumentDescriptor(httpRequest);
        else if (httpMethod == HttpMethod.Delete)
            descriptor = new DeleteRequestArgumentDescriptor(httpRequest);
        else
            throw new ArgumentOutOfRangeException("HttpMethod");

        return descriptor.GetArgument();
    }

    private static HttpMethod CastToHttpMethod(this string httpMethod)
    {
        httpMethod = httpMethod.ToLower();

        return httpMethod switch
        {
            "get" => HttpMethod.Get,
            "post" => HttpMethod.Post,
            "put" => HttpMethod.Put,
            "delete" => HttpMethod.Get,
            _ => throw new ArgumentException()
        };
    }

    
}