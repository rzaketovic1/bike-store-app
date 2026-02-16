using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    /// <summary>
    /// DTO for creating or updating a product with image upload
    /// </summary>
    public class ProductWithImageDto
    {
        // NO Id property - it comes from URL for updates, ignored for creates

        [Required(ErrorMessage = "Product name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [DefaultValue("Mountain Bike Pro")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DefaultValue("High-performance mountain bike with advanced suspension system")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        [DefaultValue(599.99)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [DefaultValue("Mountain")]
        public required string Type { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [DefaultValue("Trek")]
        public required string Brand { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10000")]
        [DefaultValue(10)]
        public int QuantityInStock { get; set; }

        /// <summary>
        /// Image file (required for create, optional for update)
        /// </summary>
        public IFormFile? Image { get; set; } // Optional for flexibility
    }
}
