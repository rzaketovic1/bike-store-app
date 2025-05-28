namespace Core.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; } // nije required
    public required string PictureUrl { get; set; }
    public required string Type { get; set; }
    public required string Brand { get; set; }
    public int QuantityInStock { get; set; } // nije required

    // Dodatna polja - bez required
    public string? FrameSize { get; set; }
    public string? WheelSize { get; set; }
    public string? Color { get; set; }
    public int? Year { get; set; }
    public bool? IsElectric { get; set; }
    public decimal? Weight { get; set; }
}
