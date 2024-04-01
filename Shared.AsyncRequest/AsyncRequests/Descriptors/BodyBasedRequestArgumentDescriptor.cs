using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Green.CT.Asyncify.AsyncRequests.Descriptors;

internal abstract class BodyBasedRequestArgumentDescriptor(
    HttpRequest httpRequest) : RequestArgumentDescriptor(httpRequest)
{
    protected override void FillArguments()
    {
        var bodyArguments = GetInputArguments(typeof(FromBodyAttribute));

        if (!bodyArguments.Any())
            return;

        var bodyArgument = bodyArguments.First();

        var body = GetBodyObject(bodyArgument.ParameterType,
            Encoding.UTF8);

        AddArgument(bodyArgument.Position, body);
    }

    private string GetRawBodyAsync(Encoding? encoding = null)
    {
        if (!Request.Body.CanSeek)
        {
            // We only do this if the stream isn't *already* seekable,
            // as EnableBuffering will create a new stream instance
            // each time it's called
            Request.EnableBuffering();
        }

        Request.Body.Position = 0;

        var reader = new StreamReader(Request.Body, encoding ?? Encoding.UTF8);

        var body = reader.ReadToEnd();

        Request.Body.Position = 0;

        return body;
    }

    private object? GetBodyObject<T>(T type, Encoding? encoding = null) where T : Type
    {
        var body = GetRawBodyAsync(encoding);

        if (string.IsNullOrEmpty(body))
            return default;

        try
        {
            return JsonConvert.DeserializeObject(body, type);
        }
        catch (Exception)
        {
            throw new InvalidCastException($"cannot cast content of body to {typeof(T)}");
        }
    }
}