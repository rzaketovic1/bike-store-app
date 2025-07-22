using Core.Dtos;

namespace Core.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDto>> GetProductsAsync(string? brand, string? type, string? sort, int pageIndex, int pageSize);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProduct(ProductDto productDto);
        Task<ProductDto> CreateProductWithImageAsync(ProductWithImageDto dto);
        Task<bool> UpdateProduct(int id, ProductDto productDto);
        Task<ProductDto?> UpdateProductWithImageAsync(ProductWithImageDto dto);
        Task<bool> DeleteProduct(int id);
        Task<IReadOnlyList<string>> GetBrandsAsync();
        Task<IReadOnlyList<string>> GetTypesAsync();
    }
}
