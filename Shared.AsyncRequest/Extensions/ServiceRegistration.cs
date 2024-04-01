using Green.CT.Asyncify.Contracts;
using Green.CT.Asyncify.Contracts.Managers;
using Green.CT.Asyncify.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Green.CT.Asyncify.Extensions;

public static class ServiceRegistration
{
    internal static void AddAsyncResponse(this WebApplication app)
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

    public static void AsyncRequest(IApplicationBuilder app)
    {
        app.UseMiddleware<AsyncRequestMiddleware>();
    }
}