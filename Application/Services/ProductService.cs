using Application.Common.Exceptions;
using Application.Common.Pagination;
using Application.Dtos;
using Application.Interfaces;
using Application.Mappings;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

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
        var dtoItems = paged.Items.Select(p => p.ToDto()).ToList();
        return new PaginatedList<ProductDto>(dtoItems, paged.TotalCount, pageIndex, pageSize);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _repo.GetProductByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }

        return product.ToDto();
    }

    public async Task<ProductDto> CreateProduct(CreateProductDto dto)
    {
        var product = dto.ToEntity();
        _repo.AddProduct(product);
        await _repo.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task<ProductDto> CreateProductWithImageAsync(ProductWithImageDto dto)
    {
        if (dto.Image == null || dto.Image.Length == 0)
            throw new BadRequestException("Image file is required when creating a product");

        if (!_fileUploadService.IsValidImage(dto.Image))
            throw new BadRequestException("Invalid image file. Allowed types: jpg, jpeg, png, gif, webp. Max size: 5MB");

        var fileName = await _fileUploadService.UploadImageAsync(dto.Image);

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

        return product.ToDto();
    }

    public async Task<ProductDto> UpdateProduct(int id, ProductDto productDto)
    {
        var existing = await _repo.GetProductByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }

        existing.Apply(productDto);

        _repo.UpdateProduct(existing);
        await _repo.SaveChangesAsync();

        return existing.ToDto();
    }

    public async Task DeleteProduct(int id)
    {
        var existing = await _repo.GetProductByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }

        if (!string.IsNullOrEmpty(existing.PictureUrl))
        {
            _fileUploadService.DeleteImage(existing.PictureUrl);
        }

        _repo.DeleteProduct(existing);
        await _repo.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync() => await _repo.GetBrandsAsync();

    public async Task<IReadOnlyList<string>> GetTypesAsync() => await _repo.GetTypesAsync();

    public async Task<ProductDto> UpdateProductWithImageAsync(int id, ProductWithImageDto dto)
    {
        var product = await _repo.GetProductByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }

        product.Apply(dto);

        if (dto.Image != null && dto.Image.Length > 0)
        {
            if (!_fileUploadService.IsValidImage(dto.Image))
                throw new BadRequestException("Invalid image file. Allowed types: jpg, jpeg, png, gif, webp. Max size: 5MB");

            if (!string.IsNullOrEmpty(product.PictureUrl))
            {
                _fileUploadService.DeleteImage(product.PictureUrl);
            }

            var fileName = await _fileUploadService.UploadImageAsync(dto.Image);
            product.PictureUrl = fileName;
        }

        _repo.UpdateProduct(product);
        await _repo.SaveChangesAsync();

        return product.ToDto();
    }
}


