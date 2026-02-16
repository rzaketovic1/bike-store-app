using Core.Dtos;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IFileUploadService _fileUploadService;

    public ProductService(IProductRepository repo, IFileUploadService fileUploadService)
    {
        _repo = repo;
        _fileUploadService = fileUploadService;
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

    public async Task<ProductDto> CreateProduct(CreateProductDto dto)
    {
        var product = dto.ToEntity();
        _repo.AddProduct(product);
        await _repo.SaveChangesAsync();
        return new ProductDto(product);
    }

    public async Task<ProductDto> CreateProductWithImageAsync(ProductWithImageDto dto)
    {
        // Image is required for create
        if (dto.Image == null || dto.Image.Length == 0)
            throw new ArgumentException("Image file is required when creating a product");

        // Validate image
        if (!_fileUploadService.IsValidImage(dto.Image))
            throw new ArgumentException("Invalid image file. Allowed types: jpg, jpeg, png, gif, webp. Max size: 5MB");

        // Upload image
        var fileName = await _fileUploadService.UploadImageAsync(dto.Image);

        // Create product
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Brand = dto.Brand,
            Type = dto.Type,
            QuantityInStock = dto.QuantityInStock,
            PictureUrl = fileName
        };

        _repo.AddProduct(product);
        await _repo.SaveChangesAsync();

        return new ProductDto(product);
    }

    public async Task<ProductDto?> UpdateProduct(int id, ProductDto productDto)
    {
        var existing = await _repo.GetProductByIdAsync(id);
        if (existing == null) return null;

        existing.Name = productDto.Name;
        existing.Description = productDto.Description;
        existing.Price = productDto.Price;
        existing.PictureUrl = productDto.PictureUrl;
        existing.Type = productDto.Type;
        existing.Brand = productDto.Brand;
        existing.QuantityInStock = productDto.QuantityInStock;

        _repo.UpdateProduct(existing);
        await _repo.SaveChangesAsync();

        return new ProductDto(existing);
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var existing = await _repo.GetProductByIdAsync(id);
        if (existing == null) return false;

        // Delete associated image
        if (!string.IsNullOrEmpty(existing.PictureUrl))
        {
            _fileUploadService.DeleteImage(existing.PictureUrl);
        }

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

    public async Task<ProductDto?> UpdateProductWithImageAsync(int id, ProductWithImageDto dto)
    {
        var product = await _repo.GetProductByIdAsync(id);
        if (product == null) return null;

        // Update basic fields
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Brand = dto.Brand;
        product.Type = dto.Type;
        product.QuantityInStock = dto.QuantityInStock;

        // Update image if provided (optional for updates)
        if (dto.Image != null && dto.Image.Length > 0)
        {
            if (!_fileUploadService.IsValidImage(dto.Image))
                throw new ArgumentException("Invalid image file. Allowed types: jpg, jpeg, png, gif, webp. Max size: 5MB");

            // Delete old image
            if (!string.IsNullOrEmpty(product.PictureUrl))
            {
                _fileUploadService.DeleteImage(product.PictureUrl);
            }

            // Upload new image
            var fileName = await _fileUploadService.UploadImageAsync(dto.Image);
            product.PictureUrl = fileName;
        }

        _repo.UpdateProduct(product);
        await _repo.SaveChangesAsync();

        return new ProductDto(product);
    }
}