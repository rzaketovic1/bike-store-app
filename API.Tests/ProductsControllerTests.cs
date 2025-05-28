using API.Controllers;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public async Task GetPaginatedProducts_ReturnsPaginatedBikeList()
        {
            // Arrange
            var products = new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Bianchi C-Sport 2.5",
                Description = "City bike with sporty geometry.",
                Price = 899,
                PictureUrl = "images/products/bianchi-csport2.5.jpg",
                Type = "City",
                Brand = "Bianchi"
            },
            new ProductDto
            {
                Id = 2,
                Name = "Cannondale Trail 5",
                Description = "Mountain bike for all terrains.",
                Price = 1200,
                PictureUrl = "images/products/cannondale-trail5.jpg",
                Type = "Mountain",
                Brand = "Cannondale"
            }
        };

            var paginated = new PaginatedList<ProductDto>(products, totalCount: 2, pageIndex: 1, pageSize: 10);

            _mockService.Setup(s =>
                s.GetProductsAsync(null, null, null, 1, 10))
                .ReturnsAsync(paginated);

            var controller = new ProductsController(_mockService.Object);

            // Act
            var result = await controller.GetPaginatedProducts(null, null, null, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value!;
            Assert.Equal(2, ((IEnumerable<ProductDto>)response.Data).Count());
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedBike_WhenSuccessful()
        {
            // Arrange
            var inputDto = new ProductDto
            {
                Name = "Trek FX 3 Disc",
                Description = "Lightweight hybrid bike for fitness and commuting.",
                Price = 1050,
                PictureUrl = "images/products/trek-fx3.jpg",
                Type = "Hybrid",
                Brand = "Trek"
            };

            var createdDto = new ProductDto
            {
                Id = 10,
                Name = inputDto.Name,
                Description = inputDto.Description,
                Price = inputDto.Price,
                PictureUrl = inputDto.PictureUrl,
                Type = inputDto.Type,
                Brand = inputDto.Brand
            };

            _mockService.Setup(s => s.CreateProduct(inputDto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.CreateProduct(inputDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<ProductDto>(createdAtResult.Value);
            Assert.Equal(createdDto.Name, returned.Name);
            Assert.Equal("GetProduct", createdAtResult.ActionName);
            Assert.Equal(createdDto.Id, createdAtResult.RouteValues["id"]);
        }
    }
}