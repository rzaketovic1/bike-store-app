using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

/// <summary>
/// Base class containing shared product properties and validation
/// </summary>
public abstract class BaseProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [DefaultValue("Mountain Bike Pro")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [DefaultValue("High-performance mountain bike with advanced suspension system")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    [DefaultValue(599.99)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Type is required")]
    [DefaultValue("Mountain")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Brand is required")]
    [DefaultValue("Trek")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10000")]
    [DefaultValue(10)]
    public int QuantityInStock { get; set; }
}
