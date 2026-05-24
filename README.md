# Asyncify.Net

[![NuGet](https://img.shields.io/nuget/v/MiladRv.Asyncify.Net.svg)](https://www.nuget.org/packages/MiladRv.Asyncify.Net)
[![Build](https://github.com/MiladRv/Asyncify.Net/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/MiladRv/Asyncify.Net/actions/workflows/publish-nuget.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![.NET](https://img.shields.io/badge/.NET-8%20%7C%2010-512BD4)

Run your heavy synchronous controller actions in the background — without changing a single line of business logic. Just add an attribute, and your endpoint immediately returns a `trackId` that the client can use to poll for the result.

```
POST /products/import  →  { "trackId": "3fa85f64..." }   (instant)
GET  /async?trackId=… →  { "status": "Pending" }
GET  /async?trackId=… →  { "status": "Complete", "result": { … } }
```

---

## Requirements

- .NET 8 or .NET 10

---

## Installation

```bash
dotnet add package Asyncify.Net
```

---

## Setup

### 1 — Register the service

In `Program.cs`, before `builder.Build()`:

```csharp
builder.Services.AddAsyncRequest();
```

### 2 — Register the middleware

After `builder.Build()`:

```csharp
app.UseAsyncRequest();
```

---

## Usage

### Step 1 — Mark the controller

Add `[AsyncController]` to the controller class and pass its own type:

```csharp
[AsyncController(typeof(ProductsController))]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [AsyncRequest(nameof(ImportProducts))]
    [HttpPost("import")]
    public IActionResult ImportProducts([FromBody] ImportRequest request)
    {
        // Heavy work goes here — no async/await needed.
        _productService.BulkImport(request.Items);
        return Ok();
    }
}
```

### Step 2 — Call the endpoint

The response comes back instantly with a `trackId`:

```json
{
  "trackId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Pending",
  "result": null
}
```

### Step 3 — Poll for the result

```
GET /async?trackId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

```json
{
  "trackId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Complete",
  "result": { ... }
}
```

---

## Request Status

| Status     | Description                                  |
|------------|----------------------------------------------|
| `Pending`  | The job has been queued but not finished yet |
| `Complete` | The job finished successfully                |
| `Failed`   | The job threw an unexpected exception        |
| `Timeout`  | The job exceeded the allowed execution time  |

---

## Endpoints added automatically

`UseAsyncRequest()` registers two endpoints on your application:

| Method | Path          | Description                                 |
|--------|---------------|---------------------------------------------|
| `GET`  | `/async`      | Poll for the result using `?trackId={guid}` |
| `GET`  | `/api/health` | Simple health check — returns `200 OK`      |

---

## Supported HTTP methods

Arguments are resolved automatically based on their binding attributes:

| HTTP Method | Argument source                      |
|-------------|--------------------------------------|
| `GET`       | `[FromQuery]`, `[FromRoute]`         |
| `POST`      | `[FromBody]`, `[FromRoute]`          |
| `PUT`       | `[FromBody]`, `[FromRoute]`          |
| `DELETE`    | `[FromRoute]`                        |

---

## How it works

1. A request hits an endpoint marked with `[AsyncRequest]`
2. The middleware intercepts it before the controller runs
3. The controller is resolved from the DI container and method arguments are extracted from the HTTP request
4. The synchronous method is dispatched to a background thread via `Task.Run`
5. A `trackId` is returned immediately to the caller
6. The caller polls `GET /async?trackId=…` to check progress and retrieve the result

---

## Contributing

Contributions are welcome. Please open an issue first to discuss what you would like to change, then submit a pull request against the `develop` branch.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes
4. Open a pull request

---

## License

Distributed under the MIT License. See [LICENSE](./LICENSE) for details.
