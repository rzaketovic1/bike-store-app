using Application.Common.Pagination;
using Application.Dtos;

namespace Application.Interfaces;

public interface IProductService
{
    Task<PaginatedList<ProductDto>> GetProductsAsync(string? brand, string? type, string? sort, int pageIndex, int pageSize);
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProduct(CreateProductDto dto);
    Task<ProductDto> CreateProductWithImageAsync(ProductWithImageDto dto);
    Task<ProductDto> UpdateProduct(int id, ProductDto dto);
    Task<ProductDto> UpdateProductWithImageAsync(int id, ProductWithImageDto dto);
    Task DeleteProduct(int id);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();
}
