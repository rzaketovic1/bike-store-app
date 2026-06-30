using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context, string contentRootPath)
    {
        var seedFilePath = GetSeedFilePath(contentRootPath);

        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync(seedFilePath);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        else
        {
            // UPDATE: Refresh PictureUrls ako su već u bazi
            var productsData = await File.ReadAllTextAsync(seedFilePath);
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

    private static string GetSeedFilePath(string contentRootPath)
    {
        return Path.Combine(contentRootPath, "Infrastructure", "Data", "SeedData", "products.json");
    }
}
