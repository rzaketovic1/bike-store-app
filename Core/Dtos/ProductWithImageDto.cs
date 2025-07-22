using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ProductWithImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Brand { get; set; } = "";
        public string Type { get; set; } = "";
        public int QuantityInStock { get; set; }
        public IFormFile? Image { get; set; }
    }
}
