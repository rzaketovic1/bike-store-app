namespace Core.Dtos;

using Core.Entities;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string PictureUrl { get; set; } = "";
    public string Type { get; set; } = "";
    public string Brand { get; set; } = "";
    public int QuantityInStock { get; set; }

    // Ako imaš dodatna polja, dodaj ih ovdje:
    // public string? FrameSize { get; set; }
    // public string? WheelSize { get; set; }
    // public string? Color { get; set; }
    // public int? Year { get; set; }
    // public bool? IsElectric { get; set; }
    // public decimal? Weight { get; set; }

    public ProductDto() { }

    public ProductDto(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        PictureUrl = "/images/products/" + product.PictureUrl;
        Type = product.Type;
        Brand = product.Brand;
        QuantityInStock = product.QuantityInStock;

        // Mapiraj i dodatna polja po potrebi:
        // FrameSize = product.FrameSize;
        // WheelSize = product.WheelSize;
        // Color = product.Color;
        // Year = product.Year;
        // IsElectric = product.IsElectric;
        // Weight = product.Weight;
    }

    public Product ToEntity()
    {
        return new Product
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Price = this.Price,
            PictureUrl = this.PictureUrl,
            Type = this.Type,
            Brand = this.Brand,
            QuantityInStock = this.QuantityInStock,

            // Mapiraj i dodatna polja po potrebi:
            // FrameSize = this.FrameSize,
            // WheelSize = this.WheelSize,
            // Color = this.Color,
            // Year = this.Year,
            // IsElectric = this.IsElectric,
            // Weight = this.Weight
        };
    }
}