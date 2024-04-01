
# Asyncify

Some times we have a api that must handle heavy request; for example, an api that adds a list of items to database and it's consumes bunch of times!

`Asyncify` will handle this problem. It will run your request in new background thread and end-user will not wait for response.



# Installation

`Asyncify` is installed from NuGet.

```dotnet add package Asyncify.Net```

## Documentation

At first, use `AsyncRequest` attribute on the request which you need to run it as async

```
[AsyncRequest(nameof(Index))]
public IActionResult Index()
```
and then add `AsyncController` attribute on controller that request is included in it.

```
[AsyncController(typeof(ProductsController))]
public class ProductsController : Controller
```

Finally, register the middleware to handle your async requests:

```
app.UseAsyncRequest();
```

For tracking response of your request, `Asyncify` will add an api with `/async` that accept `trackId` as a query and return response in this template:

```
public class AsyncRequestDto(
    Guid trackId,
    AsyncRequestStatus status,
    object? result)
{
    public Guid TrackId { get; } = trackId;
    public AsyncRequestStatus Status { get; } = status;
    public object? Result { get; } = result;
}
```

Every async request can some status that will showed in `Status` property, and the your `Dto` will filled in `Result` propery.






