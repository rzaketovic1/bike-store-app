---
applyTo: "**/*.cs,**/Program.cs,**/appsettings*.json,**/*.csproj"
description: "Use when working on the .NET backend — controllers, services, repositories, EF Core, or API configuration."
---

# Backend Instructions (.NET)

## Project Context

The backend is a .NET Web API running at `http://localhost:5000/api/`. It serves the Angular frontend with product data, authentication, and image uploads.

## Mentor Approach

Before writing any backend code:
1. Explain the .NET concept involved (e.g., dependency injection, middleware, EF Core migrations)
2. Identify which files will change and why
3. Connect the concept to how the Angular frontend consumes it

After writing code:
- Describe what changed and what it enables
- Explain how to test it (Swagger, curl, or from the Angular app)
- Point out common mistakes for that concept
- Ask one review question

## Architecture Conventions

- **Pattern**: Repository pattern + Service layer + Controllers
- **ORM**: Entity Framework Core (code-first migrations)
- **Auth**: JWT Bearer tokens — issued on login/register, validated via middleware
- **File uploads**: `IFormFile` received in controllers, stored as static files or cloud storage
- **Error responses**: Return `ProblemDetails`-compatible responses; use `BadRequest`, `NotFound`, `Unauthorized` appropriately

## Controller Conventions

- Inherit from `ControllerBase` (not `Controller` — no view support needed)
- Use `[ApiController]` and `[Route("api/[controller]")]` attributes
- Return `ActionResult<T>` or `IActionResult`
- Keep controllers thin — delegate business logic to services
- Use constructor injection for services

```csharp
// Preferred pattern
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
}
```

## Service & Repository Layer

- Define interfaces (`IProductService`, `IProductRepository`) — program to abstractions
- Register services in `Program.cs` with the correct lifetime (`AddScoped` for EF-dependent services)
- Repositories handle data access only; services handle business rules

## Entity Framework Core

- Models live in the `Core` or `Domain` layer (no EF attributes there if using Fluent API)
- Fluent API configuration goes in `EntityTypeConfiguration<T>` classes
- Migrations: `dotnet ef migrations add <Name>` then `dotnet ef database update`
- Seed data via `HasData()` in configurations or a dedicated seeder class

## Authentication (JWT)

- Token issued in `AuthController` on `/login` and `/register`
- Token payload contains `displayName` and standard claims
- Protected endpoints use `[Authorize]` attribute
- Token stored client-side in localStorage — do not store sensitive data in claims

## API Response Shapes

Paginated list response the Angular frontend expects:
```json
{
  "data": [...],
  "pageIndex": 1,
  "pageSize": 6,
  "totalCount": 24,
  "totalPages": 4
}
```

Product object shape the frontend expects:
```json
{
  "id": 1,
  "name": "...",
  "description": "...",
  "price": 0.0,
  "pictureUrl": "...",
  "type": "...",
  "brand": "...",
  "quantityInStock": 0
}
```

## Common Mistakes to Watch For

- Forgetting to register a new service in `Program.cs`
- Not adding a migration after changing a model
- Returning the wrong HTTP status code (e.g., `Ok` instead of `Created` for POST)
- Allowing circular JSON serialization (use DTOs or configure `JsonOptions`)
- Not applying `[Authorize]` to endpoints that need protection
