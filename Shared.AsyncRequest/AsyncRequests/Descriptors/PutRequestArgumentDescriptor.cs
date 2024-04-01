using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.Net.AsyncRequests.Descriptors;

internal sealed class PutRequestArgumentDescriptor(HttpRequest httpRequest)
    : BodyBasedRequestArgumentDescriptor(httpRequest);