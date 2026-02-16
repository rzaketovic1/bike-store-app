namespace Core.Dtos;

using Core.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [DefaultValue("Mountain Bike Pro")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Description is required")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
    [MaxLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
    [DefaultValue("High-performance mountain bike with advanced suspension system")]
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    [DefaultValue(599.99)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Picture URL is required")]
    [DefaultValue("bike-placeholder.png")]
    public string PictureUrl { get; set; } = "";

    [DefaultValue("Mountain")]
    public string Type { get; set; } = "";

    [DefaultValue("Trek")]
    public string Brand { get; set; } = "";

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100")]
    [DefaultValue(10)]
    public int QuantityInStock { get; set; }

    // Ako imaš dodatna polja, dodaj ih ovdje:
    // public string? FrameSize { get; set; }
    // public string? WheelSize { get; set; }
    // public string? Color { get; set; }
    // public int? Year { get; set; }
    // public bool? IsElectric { get; set; }
    // public decimal? Weight { get; set; }

    public ProductDto() { }

    /// <summary>
    /// Create DTO from Entity (for responses)
    /// </summary>
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

        // FrameSize = product.FrameSize;
        // WheelSize = product.WheelSize;
        // Color = product.Color;
        // Year = product.Year;
        // IsElectric = product.IsElectric;
        // Weight = product.Weight;
    }

    /// <summary>
    /// Convert DTO to Entity (for updates)
    /// </summary>
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

            // FrameSize = this.FrameSize,
            // WheelSize = this.WheelSize,
            // Color = this.Color,
            // Year = this.Year,
            // IsElectric = this.IsElectric,
            // Weight = this.Weight
        };
    }
}