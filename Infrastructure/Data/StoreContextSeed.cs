using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        else
        {
            // UPDATE: Refresh PictureUrls ako su već u bazi
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
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
