# Shared backend infrastructure

`Shared` contains technical capabilities that can be reused by any MedicalSmartBox bounded context. It must not contain Custody, Alerting, Identity, Transport, or Smart Container business rules.

## Available capabilities

- `AddSharedApi()` registers RFC-style Problem Details, the global exception handler, health checks, and OpenAPI generation.
- `UseSharedApi()` installs the exception handling middleware. Call it early in the HTTP pipeline.
- `MapSharedEndpoints(environment)` maps API information at `GET /`, health at `GET /health`, and (only in Development) OpenAPI at `/openapi/v1.json` plus Scalar at `/scalar`.
- `AddSharedPersistence(configuration)` configures MySQL, `AppDbContext`, `IUnitOfWork`, and the generic `IBaseRepository<TEntity, TKey>` when the `MedicalSmartBox` connection string is present. It returns `false` when no connection is configured so a development host can choose a demo adapter.
- `AppDbContext` discovers EF Core mappings from the API assembly. Add entity mappings in the bounded context that owns those entities; do not add module references to `Shared`.
- `IAuditableEntity` and its EF interceptor provide shared creation and update timestamps.

## Register a new bounded context

Keep domain-specific repository and application service registrations beside the application startup composition root. When a database connection is configured, register the bounded context's repository implementation there. In Development, a bounded context may register its own in-memory adapter when `AddSharedPersistence` returns `false`. Non-development hosts must provide a database connection; the API fails fast otherwise.

```csharp
builder.Services.AddSharedApi();
var databaseConfigured = builder.Services.AddSharedPersistence(builder.Configuration);

if (databaseConfigured)
    builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
else if (builder.Environment.IsDevelopment())
    builder.Services.AddSingleton<IExampleRepository, InMemoryExampleRepository>();
else
    throw new InvalidOperationException("A database connection is required outside Development.");

var app = builder.Build();
app.UseSharedApi();
app.MapSharedEndpoints(app.Environment);
```

Unhandled exceptions are logged with request context and returned as a generic 500 Problem Details response containing a trace identifier. Domain validation and expected business failures should be mapped to their appropriate HTTP responses by the owning bounded context, not translated to 500s by the shared handler.
