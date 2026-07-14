using System.Text.Json;
using Core.Entities;
using System.Text.Json;

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
        // Try multiple possible paths for different environments
        var possiblePaths = new[]
        {
            // Docker/Production deployment (published files with TargetPath)
            Path.Combine(contentRootPath, "Infrastructure", "Data", "SeedData", "products.json"),
            // Alternative published path
            Path.Combine(contentRootPath, "Data", "SeedData", "products.json"),
            // Development (solution structure)
            Path.Combine(Directory.GetParent(contentRootPath)?.FullName ?? contentRootPath, "Infrastructure", "Data", "SeedData", "products.json"),
            // Local project path
            Path.Combine(contentRootPath, "..", "Infrastructure", "Data", "SeedData", "products.json")
        };

        foreach (var path in possiblePaths)
        {
            var normalizedPath = Path.GetFullPath(path);
            if (File.Exists(normalizedPath))
            {
                return normalizedPath;
            }
        }

        throw new FileNotFoundException($"Could not find products.json in content root: {contentRootPath}. Searched: {string.Join(", ", possiblePaths)}");
    }
}
