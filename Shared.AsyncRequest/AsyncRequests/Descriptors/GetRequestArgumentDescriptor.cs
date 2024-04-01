using Green.CT.Asyncify.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Green.CT.Asyncify.AsyncRequests.Descriptors;

internal sealed class GetRequestArgumentDescriptor(HttpRequest httpRequest)
    : RequestArgumentDescriptor(httpRequest)
{
    protected override void FillArguments() 
    {
        var parameters = GetInputArguments(typeof(FromQueryAttribute));

        foreach (var parameter in parameters)
        {
            var value = Request
                .Query
                .FirstOrDefault(k => k.Key.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase))
                .Value
                .ToString();

            var convertedValue = ConverterExtensions.Convert(value, parameter.ParameterType);

            AddArgument(parameter.Position, convertedValue);
        }
    }

}