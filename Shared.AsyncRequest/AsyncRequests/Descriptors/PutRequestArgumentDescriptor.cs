using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.AsyncRequests.Descriptors;

internal sealed class PutRequestArgumentDescriptor(HttpRequest httpRequest)
    : BodyBasedRequestArgumentDescriptor(httpRequest);