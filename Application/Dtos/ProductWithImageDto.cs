using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace Application.Dtos;

/// <summary>
/// DTO for creating or updating a product with image file upload
/// Supports multipart/form-data requests
/// </summary>
public class ProductWithImageDto : BaseProductDto
{
    /// <summary>
    /// Image file (required for create, optional for update)
    /// </summary>
    public IFormFile? Image { get; set; }
}
