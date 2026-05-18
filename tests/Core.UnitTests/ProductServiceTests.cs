using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Core.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly Mock<IFileUploadService> _fileUploadMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _fileUploadMock = new Mock<IFileUploadService>();
        _service = new ProductService(_repoMock.Object, _fileUploadMock.Object);
    }

    #region GetProductsAsync Tests

    [Fact]
    public async Task GetProductsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var products = new List<Product>
        {
            CreateTestProduct(1, "Bike 1"),
            CreateTestProduct(2, "Bike 2")
        };
        var paginatedProducts = new PaginatedList<Product>(products, 10, 1, 2);

        _repoMock.Setup(r => r.GetProductsAsync(null, null, null, 1, 2))
            .ReturnsAsync(paginatedProducts);

        // Act
        var result = await _service.GetProductsAsync(null, null, null, 1, 2);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(10);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        // Arrange
        var paginatedProducts = new PaginatedList<Product>(new List<Product>(), 0, 1, 10);

        _repoMock.Setup(r => r.GetProductsAsync(null, null, null, 1, 10))
            .ReturnsAsync(paginatedProducts);

        // Act
        var result = await _service.GetProductsAsync(null, null, null, 1, 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region GetProductByIdAsync Tests

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProductDto_WhenProductExists()
    {
        // Arrange
        var product = CreateTestProduct(1, "Test Bike", 599.99m);

        _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Bike");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateProduct Tests

    [Fact]
    public async Task CreateProduct_ShouldAddProductAndSaveChanges()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Bike",
            Description = "A brand new bike",
            Price = 799.99m,
            PictureUrl = "bike.jpg",
            Brand = "Giant",
            Type = "Road",
            QuantityInStock = 5
        };

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.CreateProduct(dto);

        // Assert
        _repoMock.Verify(r => r.AddProduct(It.Is<Product>(p => p.Name == "New Bike")), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        result.Name.Should().Be("New Bike");
    }

    #endregion

    #region CreateProductWithImageAsync Tests

    [Fact]
    public async Task CreateProductWithImageAsync_ShouldUploadImageAndCreateProduct()
    {
        // Arrange
        var fileMock = CreateMockFormFile("bike.jpg", 1024);
        var dto = CreateProductWithImageDto("Test Bike", fileMock.Object);

        _fileUploadMock.Setup(f => f.IsValidImage(fileMock.Object)).Returns(true);
        _fileUploadMock.Setup(f => f.UploadImageAsync(fileMock.Object, "products"))
            .ReturnsAsync("unique-filename.jpg");
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.CreateProductWithImageAsync(dto);

        // Assert
        _repoMock.Verify(r => r.AddProduct(It.Is<Product>(p => p.PictureUrl == "unique-filename.jpg")), Times.Once);
        result.Name.Should().Be("Test Bike");
    }

    [Fact]
    public async Task CreateProductWithImageAsync_ShouldThrow_WhenImageIsNull()
    {
        // Arrange
        var dto = new ProductWithImageDto
        {
            Name = "Test Bike",
            Description = "A test bike description",
            Price = 599.99m,
            Brand = "Trek",
            Type = "Mountain",
            QuantityInStock = 5,
            Image = null
        };

        // Act
        var act = async () => await _service.CreateProductWithImageAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Image file is required when creating a product");
    }

    #endregion

    #region UpdateProduct Tests

    [Fact]
    public async Task UpdateProduct_ShouldUpdateAllFieldsAndSave()
    {
        // Arrange
        var existingProduct = CreateTestProduct(1, "Old Name", 499.99m);
        var updateDto = new ProductDto
        {
            Id = 1,
            Name = "New Name",
            Description = "Updated description",
            Price = 699.99m,
            PictureUrl = "new-image.jpg",
            Brand = "Giant",
            Type = "Road",
            QuantityInStock = 15
        };

        _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(existingProduct);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateProduct(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("New Name");
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNull_WhenProductNotFound()
    {
        // Arrange
        var updateDto = new ProductDto
        {
            Id = 999,
            Name = "Updated",
            Description = "Updated description",
            Price = 599.99m,
            PictureUrl = "image.jpg",
            Brand = "Trek",
            Type = "Mountain",
            QuantityInStock = 5
        };

        _repoMock.Setup(r => r.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.UpdateProduct(999, updateDto);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.UpdateProduct(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region UpdateProductWithImageAsync Tests

    [Fact]
    public async Task UpdateProductWithImageAsync_ShouldReplaceImage_WhenNewImageProvided()
    {
        // Arrange
        var existingProduct = CreateTestProduct(1, "Test Bike", 599.99m);
        existingProduct.PictureUrl = "old-image.jpg";

        var fileMock = CreateMockFormFile("new-bike.jpg", 1024);
        var dto = CreateProductWithImageDto("Updated Bike", fileMock.Object);

        _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(existingProduct);
        _fileUploadMock.Setup(f => f.IsValidImage(fileMock.Object)).Returns(true);
        _fileUploadMock.Setup(f => f.UploadImageAsync(fileMock.Object, "products"))
            .ReturnsAsync("new-unique-filename.jpg");
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateProductWithImageAsync(1, dto);

        // Assert
        _fileUploadMock.Verify(f => f.DeleteImage("old-image.jpg", "products"), Times.Once);
        existingProduct.PictureUrl.Should().Be("new-unique-filename.jpg");
    }

    [Fact]
    public async Task UpdateProductWithImageAsync_ShouldReturnNull_WhenProductNotFound()
    {
        // Arrange
        var dto = new ProductWithImageDto
        {
            Name = "Test",
            Description = "Test description",
            Price = 599.99m,
            Brand = "Trek",
            Type = "Mountain",
            QuantityInStock = 5,
            Image = null
        };

        _repoMock.Setup(r => r.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.UpdateProductWithImageAsync(999, dto);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteProduct Tests

    [Fact]
    public async Task DeleteProduct_ShouldDeleteImageAndProduct_WhenProductExists()
    {
        // Arrange
        var existingProduct = CreateTestProduct(1, "Test Bike", 599.99m);
        existingProduct.PictureUrl = "bike-image.jpg";

        _repoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(existingProduct);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteProduct(1);

        // Assert
        result.Should().BeTrue();
        _fileUploadMock.Verify(f => f.DeleteImage("bike-image.jpg", "products"), Times.Once);
        _repoMock.Verify(r => r.DeleteProduct(existingProduct), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnFalse_WhenProductNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.DeleteProduct(999);

        // Assert
        result.Should().BeFalse();
        _repoMock.Verify(r => r.DeleteProduct(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region GetBrandsAsync Tests

    [Fact]
    public async Task GetBrandsAsync_ShouldReturnBrandsFromRepository()
    {
        // Arrange
        var brands = new List<string> { "Trek", "Giant", "Specialized" };
        _repoMock.Setup(r => r.GetBrandsAsync()).ReturnsAsync(brands);

        // Act
        var result = await _service.GetBrandsAsync();

        // Assert
        result.Should().BeEquivalentTo(brands);
    }

    [Fact]
    public async Task GetBrandsAsync_ShouldReturnEmptyList_WhenNoBrandsExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetBrandsAsync()).ReturnsAsync(new List<string>());

        // Act
        var result = await _service.GetBrandsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetTypesAsync Tests

    [Fact]
    public async Task GetTypesAsync_ShouldReturnTypesFromRepository()
    {
        // Arrange
        var types = new List<string> { "Mountain", "Road", "Hybrid" };
        _repoMock.Setup(r => r.GetTypesAsync()).ReturnsAsync(types);

        // Act
        var result = await _service.GetTypesAsync();

        // Assert
        result.Should().BeEquivalentTo(types);
    }

    [Fact]
    public async Task GetTypesAsync_ShouldReturnEmptyList_WhenNoTypesExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetTypesAsync()).ReturnsAsync(new List<string>());

        // Act
        var result = await _service.GetTypesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static Product CreateTestProduct(int id, string name, decimal price = 599.99m) => new()
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

    private static Mock<IFormFile> CreateMockFormFile(string fileName, long length)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(length);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        return fileMock;
    }

    private static ProductWithImageDto CreateProductWithImageDto(string name, IFormFile image) => new()
    {
        Name = name,
        Description = "Test product description",
        Price = 599.99m,
        Brand = "Trek",
        Type = "Mountain",
        QuantityInStock = 10,
        Image = image
    };

    #endregion
}
