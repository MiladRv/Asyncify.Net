using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.Net.AsyncRequests.Descriptors;

internal class PostRequestArgumentDescriptor(HttpRequest httpRequest)
    : BodyBasedRequestArgumentDescriptor(httpRequest);