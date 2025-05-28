using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedList<ProductDto>> GetProductsAsync(string? brand, string? type, string? sort, int pageIndex, int pageSize)
    {
        var paged = await _repo.GetProductsAsync(brand, type, sort, pageIndex, pageSize);
        var dtoItems = paged.Items.Select(p => new ProductDto(p)).ToList();
        return new PaginatedList<ProductDto>(dtoItems, paged.TotalCount, pageIndex, pageSize);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repo.GetProductByIdAsync(id);
        return product == null ? null : new ProductDto(product);
    }

    public async Task<ProductDto> CreateProduct(ProductDto productDto)
    {
        var product = productDto.ToEntity();
        _repo.AddProduct(product);
        await _repo.SaveChangesAsync();
        return new ProductDto(product);
    }

    public async Task<bool> UpdateProduct(int id, ProductDto productDto)
    {
        var existing = await _repo.GetProductByIdAsync(id);
        if (existing == null) return false;
        // Mapiraj polja
        existing.Name = productDto.Name;
        existing.Description = productDto.Description;
        existing.Price = productDto.Price;
        existing.PictureUrl = productDto.PictureUrl;
        existing.Type = productDto.Type;
        existing.Brand = productDto.Brand;
        existing.QuantityInStock = productDto.QuantityInStock;
        // Mapiraj ostala polja ako ih imaš
        _repo.UpdateProduct(existing);
        return await _repo.SaveChangesAsync();
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var existing = await _repo.GetProductByIdAsync(id);
        if (existing == null) return false;
        _repo.DeleteProduct(existing);
        return await _repo.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _repo.GetBrandsAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await _repo.GetTypesAsync();
    }
}