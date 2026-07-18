# Unit Test Naming Convention

This document defines the official naming standards for all unit tests in the BikeStore application.

---

## Test Class Naming

### Pattern
```
{ClassUnderTest}Tests
```

### Examples
- `ProductsControllerTests` ✅
- `UserServiceTests` ✅
- `TokenServiceTests` ✅
- `ErrorHandlingMiddlewareTests` ✅

### Rules
- Always use the **full class name** being tested
- Always append `Tests` suffix
- Use PascalCase
- Place in namespace matching the layer: `API.UnitTests`, `Application.UnitTests`, `Infrastructure.UnitTests`

---

## Test Method Naming

### Pattern
```
{MethodName}_{ExpectedBehavior}_{StateUnderTest}
```

### Components Explained

1. **MethodName**: The method being tested (use exact name from class under test)
2. **ExpectedBehavior**: What should happen (use `Should` prefix for clarity)
3. **StateUnderTest**: The condition/scenario (optional but recommended for clarity)

### Examples

#### ✅ Good Examples
```csharp
// Controllers
[Fact]
public async Task GetPaginatedProducts_ShouldReturnOk_WithPaginatedData()

[Fact]
public async Task GetProduct_ShouldReturnOk_WhenProductExists()

[Fact]
public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()

[Fact]
public async Task CreateProduct_ShouldReturnCreated_WithValidData()

[Fact]
public async Task CreateProduct_ShouldReturnBadRequest_WhenModelIsInvalid()

// Services
[Fact]
public async Task CreateAsync_ShouldCreateUserAndSaveChanges()

[Fact]
public async Task CreateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()

[Fact]
public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()

[Fact]
public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()

// Middleware
[Fact]
public async Task InvokeAsync_ShouldMapNotFoundException_ToProblemDetails()

[Fact]
public async Task InvokeAsync_ShouldMapConflictException_ToProblemDetails()
```

#### ❌ Bad Examples
```csharp
[Fact]
public async Task Test1() // Too vague

[Fact]
public async Task GetProduct_ReturnsOk() // Missing state context

[Fact]
public async Task TestGetProductWhenExists() // Not following pattern

[Fact]
public async Task get_product_when_exists() // Wrong casing (snake_case)
```

---

## Test Structure

### Follow Arrange-Act-Assert (AAA)

Always structure tests using the AAA pattern with clear comments:

```csharp
[Fact]
public async Task GetProduct_ShouldReturnOk_WhenProductExists()
{
    // Arrange
    var product = CreateProductDto(1, "Test Bike", 599.99m);
    _serviceMock.Setup(s => s.GetProductByIdAsync(1))
        .ReturnsAsync(product);

    // Act
    var result = await _controller.GetProduct(1);

    // Assert
    var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(product);
}
```

### Use Region Comments for Organization

Group related tests using regions:

```csharp
#region GetPaginatedProducts Tests

[Fact]
public async Task GetPaginatedProducts_ShouldReturnOk_WithPaginatedData()
{
    // ...
}

[Fact]
public async Task GetPaginatedProducts_ShouldReturnOk_WithFilters()
{
    // ...
}

#endregion

#region GetProduct Tests

[Fact]
public async Task GetProduct_ShouldReturnOk_WhenProductExists()
{
    // ...
}

[Fact]
public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
{
    // ...
}

#endregion
```

---

## Theory Tests with InlineData

When using `[Theory]` with multiple test cases:

```csharp
[Theory]
[InlineData(null, null, null)] // All filters null
[InlineData("Trek", null, null)] // Brand filter only
[InlineData(null, "Mountain", null)] // Type filter only
[InlineData(null, null, "bike")] // Search term only
public async Task GetPaginatedProducts_ShouldReturnOk_WithVariousFilters(
    string? brand, string? type, string? search)
{
    // Arrange
    var products = new List<ProductDto> { CreateProductDto(1, "Bike", 499.99m) };
    var paginatedResult = new PaginatedList<ProductDto>(products, 1, 1, 1);

    _serviceMock.Setup(s => s.GetProductsAsync(brand, type, search, 1, 10))
        .ReturnsAsync(paginatedResult);

    // Act
    var result = await _controller.GetPaginatedProducts(brand, type, search, 1, 10);

    // Assert
    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.StatusCode.Should().Be(200);
}
```

**Name these tests clearly:**
```csharp
{MethodName}_ShouldReturnExpectedResult_WithVariousInputs
```

---

## Assertions

### Use FluentAssertions

Always prefer FluentAssertions for readability:

```csharp
// ✅ Good
result.Should().BeOfType<OkObjectResult>();
okResult.StatusCode.Should().Be(200);
response.Email.Should().Be("test@example.com");
data.Should().HaveCount(2);

// ❌ Avoid
Assert.IsType<OkObjectResult>(result);
Assert.Equal(200, okResult.StatusCode);
Assert.Equal("test@example.com", response.Email);
```

---

## Mock Naming

Use descriptive mock field names:

```csharp
// ✅ Good
private readonly Mock<IProductService> _serviceMock;
private readonly Mock<IProductRepository> _repositoryMock;
private readonly Mock<ITokenService> _tokenServiceMock;

// ❌ Bad
private readonly Mock<IProductService> _mock;
private readonly Mock<IProductService> _m;
private readonly Mock<IProductService> productServiceMock; // Should use underscore prefix
```

---

## Helper Methods

### Naming
Use descriptive names in PascalCase:

```csharp
private static ProductDto CreateProductDto(int id, string name, decimal price)
{
    return new ProductDto
    {
        Id = id,
        Name = name,
        Price = price,
        // ... other properties
    };
}

private static DefaultHttpContext CreateHttpContextWithPath(string path)
{
    var context = new DefaultHttpContext();
    context.Request.Path = path;
    return context;
}
```

---

## Test-Specific Naming Patterns

### Controllers
```
{ActionName}_Should{Outcome}_{Condition}
```

Examples:
- `GetPaginatedProducts_ShouldReturnOk_WithPaginatedData`
- `CreateProduct_ShouldReturnCreated_WithValidData`
- `UpdateProduct_ShouldReturnNoContent_WhenUpdateSucceeds`
- `DeleteProduct_ShouldReturnNotFound_WhenProductDoesNotExist`

### Services
```
{MethodName}_Should{Outcome}_{Condition}
```

Examples:
- `CreateAsync_ShouldCreateUserAndSaveChanges`
- `AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid`
- `GetProductsAsync_ShouldReturnFilteredProducts_WhenFiltersProvided`

### Repositories
```
{MethodName}_Should{Outcome}_{Condition}
```

Examples:
- `GetByIdAsync_ShouldReturnProduct_WhenProductExists`
- `AddAsync_ShouldAddProductToContext`
- `DeleteAsync_ShouldRemoveProduct_WhenProductExists`

### Middleware
```
{MethodName}_Should{Outcome}_{ExceptionType}
```

Examples:
- `InvokeAsync_ShouldMapNotFoundException_ToProblemDetails`
- `InvokeAsync_ShouldMapConflictException_ToProblemDetails`
- `InvokeAsync_ShouldReturn500_ForUnhandledExceptions`

---

## Common State Descriptors

Use these consistent terms for common scenarios:

| Scenario | Use This |
|----------|----------|
| Valid input | `WithValidData` |
| Invalid input | `WithInvalidData` / `WhenModelIsInvalid` |
| Entity exists | `WhenEntityExists` / `WhenProductExists` |
| Entity not found | `WhenEntityDoesNotExist` / `WhenProductNotFound` |
| Successful operation | `WhenOperationSucceeds` |
| Failed operation | `WhenOperationFails` |
| Authorized user | `WhenUserIsAuthorized` |
| Unauthorized user | `WhenUserIsUnauthorized` |
| Empty collection | `WhenCollectionIsEmpty` |
| Null value | `WhenValueIsNull` |

---

## Common Expected Behaviors

Use these consistent verbs:

| Action | Use This |
|--------|----------|
| Returns status | `ShouldReturnOk`, `ShouldReturnCreated`, `ShouldReturnNotFound` |
| Returns data | `ShouldReturnData`, `ShouldReturnPaginatedData` |
| Throws exception | `ShouldThrowException`, `ShouldThrowNotFoundException` |
| Calls method | `ShouldCallMethod`, `ShouldInvokeRepository` |
| Creates entity | `ShouldCreateEntity`, `ShouldCreateUser` |
| Updates entity | `ShouldUpdateEntity`, `ShouldUpdateProduct` |
| Deletes entity | `ShouldDeleteEntity`, `ShouldRemoveProduct` |
| Maps/Converts | `ShouldMapToDto`, `ShouldConvertToEntity` |

---

## File Organization

```
tests/
├── API.UnitTests/
│   ├── ProductsControllerTests.cs
│   ├── AuthControllerTests.cs
│   └── ErrorHandlingMiddlewareTests.cs
├── Application.UnitTests/
│   ├── ProductServiceTests.cs
│   └── UserServiceTests.cs
└── Infrastructure.UnitTests/
    ├── TokenServiceTests.cs
    └── FileUploadServiceTests.cs
```

---

## Summary Checklist

Before committing unit tests, verify:

- ✅ Class name follows `{ClassUnderTest}Tests` pattern
- ✅ Method name follows `{MethodName}_Should{Outcome}_{Condition}` pattern
- ✅ Uses Arrange-Act-Assert structure with comments
- ✅ Groups related tests with `#region` comments
- ✅ Uses FluentAssertions for all assertions
- ✅ Mock fields use `_fieldNameMock` naming
- ✅ Helper methods are descriptive and in PascalCase
- ✅ Test covers both happy path and edge cases

---

## References

- **xUnit Documentation**: https://xunit.net/
- **FluentAssertions**: https://fluentassertions.com/
- **Moq**: https://github.com/moq/moq4

---

**Last Updated**: 2026-07-18  
**Maintainer**: Ramiz Zaketović
