using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

/// <summary>
/// DTO for creating a product with a URL-based image reference
/// </summary>
public class CreateProductDto : BaseProductDto
{
    [Required(ErrorMessage = "Picture URL is required")]
    [DefaultValue("bike-placeholder.png")]
    public string PictureUrl { get; set; } = string.Empty;
}
