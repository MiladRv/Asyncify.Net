using Microsoft.AspNetCore.Http;

namespace Green.CT.Asyncify.Net.AsyncRequests.Descriptors;

internal sealed class DeleteRequestArgumentDescriptor(HttpRequest httpRequest)
    : RequestArgumentDescriptor(httpRequest);