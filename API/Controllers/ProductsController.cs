using Core.Dtos;
using Core.Interfaces;
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

        if (product == null)
            return NotFound(new { message = $"Product with ID {id} not found" });

        return Ok(product);
    }

    /// <summary>
    /// Get list of all available product brands
    /// </summary>
    /// <response code="200">Returns list of brand names</response>
    /// <remarks>
    /// Returns a distinct list of all brands currently in the product catalog.
    /// Useful for populating filter dropdowns in the UI.
    /// </remarks>
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
    /// <response code="200">Returns list of product type names</response>
    /// <remarks>
    /// Returns a distinct list of all product types currently in the catalog.
    /// Useful for populating filter dropdowns in the UI.
    /// Examples: Mountain, Road, Hybrid, Electric, etc.
    /// </remarks>
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
    /// <param name="dto">Product details</param>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    /// <response code="401">Unauthorized - authentication required</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await service.CreateProduct(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    /// <summary>
    /// Create a new product with image upload (requires authentication)
    /// </summary>
    /// <param name="dto">Product details with image file</param>
    /// <response code="201">Product created successfully with uploaded image</response>
    /// <response code="400">Invalid product data or image file</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <remarks>
    /// Accepts multipart/form-data with:
    /// - Image file (required): jpg, jpeg, png, gif, webp - Max 5MB
    /// - Product details (all required fields)
    /// </remarks>
    [HttpPost("with-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductDto>> CreateProductWithImage([FromForm] ProductWithImageDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Image == null || dto.Image.Length == 0)
            return BadRequest(new { message = "Image file is required when creating a product" });

        try
        {
            var created = await service.CreateProductWithImageAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (IOException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to upload image", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product without changing the image (requires authentication)
    /// </summary>
    /// <param name="id">Product ID from URL</param>
    /// <param name="dto">Updated product details</param>
    /// <response code="200">Product updated successfully, returns updated product</response>
    /// <response code="400">Invalid product data or ID mismatch</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Product not found</response>
    /// <remarks>
    /// Updates product details while preserving the existing image.
    /// 
    /// **Note:** The product ID must match in both the URL and request body.
    /// 
    /// To update the product image, use: PUT /api/Products/{id}/with-image
    /// </remarks>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, ProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch between URL and request body" });

        var updated = await service.UpdateProduct(id, dto);
        if (updated == null)
            return NotFound(new { message = $"Product with ID {id} not found" });

        return Ok(updated);
    }

    /// <summary>
    /// Update an existing product with optional new image (requires authentication)
    /// </summary>
    /// <param name="id">Product ID from URL</param>
    /// <param name="dto">Updated product details with optional image file</param>
    /// <response code="200">Product updated successfully</response>
    /// <response code="400">Invalid product data or image file</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Product not found</response>
    /// <remarks>
    /// The product ID comes from the URL, not the request body.
    /// 
    /// **Image Behavior:**
    /// - If Image is NOT provided or empty → Existing image is kept
    /// - If Image is provided with a file → New image replaces the old one
    /// </remarks>
    [HttpPut("{id}/with-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProductWithImage(int id, [FromForm] ProductWithImageDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await service.UpdateProductWithImageAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (IOException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to upload image", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a product (requires authentication)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var deleted = await service.DeleteProduct(id);
        if (!deleted)
            return NotFound(new { message = $"Product with ID {id} not found" });

        return NoContent();
    }
}