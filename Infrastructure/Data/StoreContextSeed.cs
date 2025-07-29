using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "products.json");
            var productsData = await File.ReadAllTextAsync(path);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        else
        {
            // UPDATE: Refresh PictureUrls ako su već u bazi
            var path = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "products.json");
            var productsData = await File.ReadAllTextAsync(path);
            var updatedProducts = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (updatedProducts == null) return;

            foreach (var updated in updatedProducts)
            {
                var existing = context.Products.FirstOrDefault(p => p.Name == updated.Name);
                if (existing != null && existing.PictureUrl != updated.PictureUrl)
                {
                    existing.PictureUrl = updated.PictureUrl;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
