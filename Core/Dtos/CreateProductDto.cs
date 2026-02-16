using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class CreateProductDto
    {
        // NO Id property - database will generate it!

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

        /// <summary>
        /// Convert DTO to Entity for database operations
        /// </summary>
        public Product ToEntity()
        {
            return new Product
            {
                Name = this.Name,
                Description = this.Description,
                Price = this.Price,
                PictureUrl = this.PictureUrl,
                Type = this.Type,
                Brand = this.Brand,
                QuantityInStock = this.QuantityInStock
            };
        }
    }
}
