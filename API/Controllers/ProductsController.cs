using Application.Common.Exceptions;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(IProductService service) : ControllerBase
{
    /// <summary>
    /// Get paginated list of products with optional filtering and sorting
    /// </summary>
    /// <param name="brand">Filter by brand name</param>
    /// <param name="type">Filter by product type</param>
    /// <param name="sort">Sort order: priceAsc, priceDesc, name</param>
    /// <param name="pageIndex">Page number (starts from 1)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <response code="200">Returns paginated product list</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPaginatedProducts(
        string? brand, string? type, string? sort, int pageIndex = 1, int pageSize = 10)
    {
        var paginatedResult = await service.GetProductsAsync(brand, type, sort, pageIndex, pageSize);

        return Ok(new
        {
            paginatedResult.PageIndex,
            paginatedResult.PageSize,
            paginatedResult.TotalCount,
            paginatedResult.TotalPages,
            Data = paginatedResult.Items
        });
    }

    /// <summary>
    /// Get a single product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <response code="200">Returns the product details</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await service.GetProductByIdAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// Get list of all available product brands
    /// </summary>
    [HttpGet("brands")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var brands = await service.GetBrandsAsync();
        return Ok(brands);
    }

    /// <summary>
    /// Get list of all available product types
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var types = await service.GetTypesAsync();
        return Ok(types);
    }

    /// <summary>
    /// Create a new product (requires authentication)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var created = await service.CreateProduct(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    /// <summary>
    /// Create a new product with image upload (requires authentication)
    /// </summary>
    [HttpPost("with-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductDto>> CreateProductWithImage([FromForm] ProductWithImageDto dto)
    {
        var created = await service.CreateProductWithImageAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing product without changing the image (requires authentication)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, ProductDto dto)
    {
        if (id != dto.Id)
            throw new BadRequestException("ID mismatch between URL and request body");

        var updated = await service.UpdateProduct(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Update an existing product with optional new image (requires authentication)
    /// </summary>
    [HttpPut("{id}/with-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProductWithImage(int id, [FromForm] ProductWithImageDto dto)
    {
        var updated = await service.UpdateProductWithImageAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Delete a product (requires authentication)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        await service.DeleteProduct(id);
        return NoContent();
    }
}
