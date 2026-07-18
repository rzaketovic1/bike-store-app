using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

/// <summary>
/// DTO for reading and updating products (includes ID and picture URL)
/// </summary>
public class ProductDto : BaseProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Picture URL is required")]
    [DefaultValue("bike-placeholder.png")]
    public string PictureUrl { get; set; } = string.Empty;
}
