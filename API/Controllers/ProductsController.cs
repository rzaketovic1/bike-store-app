using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await service.GetProductByIdAsync(id);

        if (product == null) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto dto)
    {
        var created = await service.CreateProduct(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, ProductDto dto)
    {
        var updated = await service.UpdateProduct(id, dto);
        if (!updated) return BadRequest("Cannot update this product");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var deleted = await service.DeleteProduct(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await service.GetBrandsAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await service.GetTypesAsync());
    }
}
