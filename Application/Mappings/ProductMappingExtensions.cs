using Application.Dtos;
using Core.Entities;

namespace Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            PictureUrl = "/images/products/" + product.PictureUrl,
            Type = product.Type,
            Brand = product.Brand,
            QuantityInStock = product.QuantityInStock
        };
    }

    public static Product ToEntity(this CreateProductDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            PictureUrl = dto.PictureUrl,
            Type = dto.Type,
            Brand = dto.Brand,
            QuantityInStock = dto.QuantityInStock
        };
    }

    public static void Apply(this Product product, ProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.PictureUrl = dto.PictureUrl;
        product.Type = dto.Type;
        product.Brand = dto.Brand;
        product.QuantityInStock = dto.QuantityInStock;
    }

    public static void Apply(this Product product, ProductWithImageDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Type = dto.Type;
        product.Brand = dto.Brand;
        product.QuantityInStock = dto.QuantityInStock;
    }
}
