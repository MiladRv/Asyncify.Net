using Green.CT.Asyncify.Net.Contracts;
using Green.CT.Asyncify.Net.Contracts.Managers;
using Green.CT.Asyncify.Net.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Green.CT.Asyncify.Net.Extensions;

public static class ServiceRegistration
{
    private static void AddAsyncResponse(this WebApplication app)
    {
        var asyncRequestManager = app.Services.GetRequiredService<IAsyncRequestManager>();

        app.MapGet("/api/health", async context =>
                await context.Response.WriteAsync("My API is healthy!"))
            .WithName("HealthCheck")
            .WithDisplayName("HealthCheck");

        app
            .MapGet("/async", async context =>
            {
                var query = context.Request.Query["trackId"];

                if (!Guid.TryParse(query, out var requestId))
                    throw new BadHttpRequestException($"requestId type is not valid");

                var asyncRequest = asyncRequestManager.GetResult(requestId);

                await context.Response.WriteAsync(JsonConvert.SerializeObject(asyncRequest));
            })
            .WithName("async")
            .WithDescription("Some Method Description")
            .WithOpenApi();
    }

    public static IApplicationBuilder UseAsyncRequest(this WebApplication app)
    {

        app.UseWhen(context => context?.GetEndpoint() != null &&
                               context.GetEndpoint()!.Metadata.OfType<AsyncRequestAttribute>().Any(),
            AsyncRequest);

        app.AddAsyncResponse();

        return app;
    }

    private static void AsyncRequest(IApplicationBuilder app)
    {
        app.UseMiddleware<AsyncRequestMiddleware>();
    }
}