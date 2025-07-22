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

    public async Task<ProductDto> CreateProductWithImageAsync(ProductWithImageDto dto)
    {
        if (dto.Image == null || dto.Image.Length == 0)
            throw new ArgumentException("Image file is required");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Path.GetFileName(dto.Image.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.Image.CopyToAsync(stream);
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Brand = dto.Brand,
            Type = dto.Type,
            QuantityInStock = dto.QuantityInStock,
            PictureUrl = fileName // samo naziv fajla
        };

        _repo.AddProduct(product);
        await _repo.SaveChangesAsync();

        return new ProductDto(product);
    }

    public async Task<ProductDto?> UpdateProductWithImageAsync(ProductWithImageDto dto)
    {
        var product = await _repo.GetProductByIdAsync(dto.Id);
        if (product == null) return null;

        // Update polja
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Brand = dto.Brand;
        product.Type = dto.Type;
        product.QuantityInStock = dto.QuantityInStock;

        // Ako je stigla nova slika
        if (dto.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
            var filePath = Path.Combine("wwwroot/images/products", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            product.PictureUrl = fileName;
        }

        _repo.UpdateProduct(product);
        await _repo.SaveChangesAsync();

        return new ProductDto(product);
    }
}