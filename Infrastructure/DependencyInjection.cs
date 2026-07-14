using Application.Interfaces;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Parse if it's a URI format (Supabase/Heroku style)
        if (connectionString?.StartsWith("postgresql://") == true)
        {
            connectionString = ParsePostgresUri(connectionString);
        }

        services.AddDbContext<StoreContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileUploadService, FileUploadService>();

        return services;
    }

    private static string ParsePostgresUri(string uri)
    {
        try
        {
            var parsedUri = new Uri(uri);
            var userInfo = parsedUri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
            var host = parsedUri.Host;
            var port = parsedUri.Port > 0 ? parsedUri.Port : 5432;
            var database = parsedUri.AbsolutePath.TrimStart('/');

            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch
        {
            // If parsing fails, return the original string
            return uri;
        }
    }
}
