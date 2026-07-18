using API.Controllers;
using Application.Common.Exceptions;
using Application.Common.Pagination;
using Application.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.UnitTests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);
    }

    #region GetPaginatedProducts Tests

    [Fact]
    public async Task GetPaginatedProducts_ShouldReturnOk_WithPaginatedData()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            CreateProductDto(1, "Bike 1", 499.99m),
            CreateProductDto(2, "Bike 2", 699.99m)
        };
        var paginatedResult = new PaginatedList<ProductDto>(products, 10, 1, 2);

        _serviceMock.Setup(s => s.GetProductsAsync(null, null, null, 1, 10))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetPaginatedProducts(null, null, null, 1, 10);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var value = okResult.Value;
        value.Should().NotBeNull();

        var pageIndex = value!.GetType().GetProperty("PageIndex")?.GetValue(value);
        var pageSize = value.GetType().GetProperty("PageSize")?.GetValue(value);
        var totalCount = value.GetType().GetProperty("TotalCount")?.GetValue(value);
        var totalPages = value.GetType().GetProperty("TotalPages")?.GetValue(value);
        var data = value.GetType().GetProperty("Data")?.GetValue(value) as IReadOnlyList<ProductDto>;

        pageIndex.Should().Be(1);
        pageSize.Should().Be(2);
        totalCount.Should().Be(10);
        totalPages.Should().Be(5);
        data.Should().HaveCount(2);
        data![0].Name.Should().Be("Bike 1");
        data[0].Price.Should().Be(499.99m);
        data[1].Name.Should().Be("Bike 2");
        data[1].Price.Should().Be(699.99m);
    }
    #endregion

    #region GetProduct Tests

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
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;

        returnedProduct.Id.Should().Be(1);
        returnedProduct.Name.Should().Be("Test Bike");
        returnedProduct.Price.Should().Be(599.99m);
    }

    #endregion

    #region GetBrands Tests

    [Fact]
    public async Task GetBrands_ShouldReturnOk_WithBrandsList()
    {
        // Arrange
        var brands = new List<string> { "Trek", "Giant", "Specialized" };

        _serviceMock.Setup(s => s.GetBrandsAsync())
            .ReturnsAsync(brands);

        // Act
        var result = await _controller.GetBrands();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBrands = okResult.Value.Should().BeAssignableTo<IReadOnlyList<string>>().Subject;

        returnedBrands.Should().HaveCount(3);
        returnedBrands.Should().Contain("Trek");
        returnedBrands.Should().Contain("Giant");
        returnedBrands.Should().Contain("Specialized");
    }

    #endregion

    #region GetTypes Tests

    [Fact]
    public async Task GetTypes_ShouldReturnOk_WithTypesList()
    {
        // Arrange
        var types = new List<string> { "Mountain", "Road", "Hybrid" };

        _serviceMock.Setup(s => s.GetTypesAsync())
            .ReturnsAsync(types);

        // Act
        var result = await _controller.GetTypes();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypes = okResult.Value.Should().BeAssignableTo<IReadOnlyList<string>>().Subject;

        returnedTypes.Should().HaveCount(3);
        returnedTypes.Should().Contain("Mountain");
        returnedTypes.Should().Contain("Road");
        returnedTypes.Should().Contain("Hybrid");
    }

    #endregion

    #region CreateProduct Tests

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Bike",
            Description = "A brand new bike",
            Price = 799.99m,
            PictureUrl = "bike.jpg",
            Brand = "Trek",
            Type = "Mountain",
            QuantityInStock = 10
        };

        var createdProduct = CreateProductDto(1, "New Bike", 799.99m);

        _serviceMock.Setup(s => s.CreateProduct(createDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(_controller.GetProduct));

        var returnedProduct = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be("New Bike");
        returnedProduct.Price.Should().Be(799.99m);
        returnedProduct.Id.Should().Be(1);
    }

    #endregion

    #region CreateProductWithImage Tests

    [Fact]
    public async Task CreateProductWithImage_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.FileName).Returns("bike.jpg");

        var dto = new ProductWithImageDto
        {
            Name = "New Bike",
            Description = "A brand new bike",
            Price = 899.99m,
            Brand = "Trek",
            Type = "Mountain",
            QuantityInStock = 5,
            Image = fileMock.Object
        };

        var createdProduct = CreateProductDto(1, "New Bike", 899.99m);

        _serviceMock.Setup(s => s.CreateProductWithImageAsync(dto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProductWithImage(dto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
    }

    #endregion

    #region UpdateProduct Tests

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenUpdateSuccessful()
    {
        // Arrange
        var updateDto = CreateProductDto(1, "Updated Bike", 699.99m);

        _serviceMock.Setup(s => s.UpdateProduct(1, updateDto))
            .ReturnsAsync(updateDto);

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;

        returnedProduct.Name.Should().Be("Updated Bike");
        returnedProduct.Price.Should().Be(699.99m);
    }

    [Fact]
    public async Task UpdateProduct_ShouldThrowBadRequestException_WhenIdMismatch()
    {
        // Arrange
        var updateDto = CreateProductDto(2, "Test Bike");

        // Act
        var act = async () => await _controller.UpdateProduct(1, updateDto);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("ID mismatch between URL and request body");
    }

    #endregion

    #region UpdateProductWithImage Tests

    [Fact]
    public async Task UpdateProductWithImage_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var dto = new ProductWithImageDto
        {
            Name = "Updated Bike",
            Description = "Updated description",
            Price = 799.99m,
            Brand = "Giant",
            Type = "Road",
            QuantityInStock = 15,
            Image = null
        };

        var updatedProduct = CreateProductDto(1, "Updated Bike", 799.99m);

        _serviceMock.Setup(s => s.UpdateProductWithImageAsync(1, dto))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProductWithImage(1, dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;

        returnedProduct.Name.Should().Be("Updated Bike");
    }

    #endregion

    #region DeleteProduct Tests

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenDeleteSuccessful()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteProduct(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    #endregion

    #region Helper Methods

    private static ProductDto CreateProductDto(int id, string name, decimal price = 599.99m) => new()
    {
        Id = id,
        Name = name,
        Description = "Test product description",
        Price = price,
        PictureUrl = "test-image.jpg",
        Brand = "Trek",
        Type = "Mountain",
        QuantityInStock = 10
    };

    #endregion
}

