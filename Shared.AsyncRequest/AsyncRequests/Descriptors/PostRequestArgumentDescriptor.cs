using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.AsyncRequests.Descriptors;

internal class PostRequestArgumentDescriptor(HttpRequest httpRequest)
    : BodyBasedRequestArgumentDescriptor(httpRequest);