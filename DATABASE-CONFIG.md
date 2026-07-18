# Database Configuration Guide

This project supports both **local Docker PostgreSQL** and **Supabase** for development.

## Initial Setup

### First Time Setup (Supabase)
If you want to use Supabase:

1. Copy the template file:
   ```sh
   cp API/appsettings.Supabase.json.template API/appsettings.Supabase.json
   ```

2. Edit `API/appsettings.Supabase.json` with your Supabase credentials

3. The file is already in `.gitignore` and won't be committed

---

## Quick Start

### Option 1: Local Docker (Default)
```sh
# Start PostgreSQL container
docker-compose up -d

# Run with Development environment (uses local Docker)
dotnet run --project API
```

### Option 2: Supabase
```sh
# Run with Supabase environment
dotnet run --project API --environment Supabase
```

---

## Configuration Files

| File | Environment | Database |
|------|-------------|----------|
| `appsettings.Development.json` | Development | Local Docker PostgreSQL |
| `appsettings.Supabase.json` | Supabase | Supabase PostgreSQL |

---

## Switching Between Environments

### Using CLI:

**Local Docker:**
```sh
dotnet run --project API --environment Development
```

**Supabase:**
```sh
dotnet run --project API --environment Supabase
```

### Using Visual Studio:

1. In the toolbar, you'll see a dropdown with launch profiles:
   - **http** → Uses local Docker
   - **http (Supabase)** → Uses Supabase

2. Select the profile you want and click Run (F5)

### Using Visual Studio (Alternative):

1. Right-click on the **API** project
2. Select **Properties**
3. Go to **Debug** → **General** → **Open debug launch profiles UI**
4. Set `ASPNETCORE_ENVIRONMENT` to either:
   - `Development` (for Docker)
   - `Supabase` (for Supabase)

### Using VS Code (launch.json):

Add multiple configurations:

```json
{
  "configurations": [
    {
      "name": "API (Docker)",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    {
      "name": "API (Supabase)",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Supabase"
      }
    }
  ]
}
```

---

## Database Migrations

### Local Docker:
```sh
# Apply migrations
dotnet ef database update --project Infrastructure --startup-project API

# Create new migration
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
```

### Supabase:
```sh
# Apply migrations to Supabase
dotnet run --project API --environment Supabase
dotnet ef database update --project Infrastructure --startup-project API -- --environment Supabase
```

---

## Connection Strings

### Local Docker (Development)
```
Host=localhost;Port=5432;Database=bikestoredb;Username=postgres;Password=postgres
```

### Supabase
```
Host=aws-1-eu-west-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.pwvsaualybizgvaymmwb;Password=supabase!123;SSL Mode=Require;Trust Server Certificate=true
```

---

## Docker Commands

```sh
# Start container
docker-compose up -d

# View logs
docker-compose logs -f postgres-db

# Stop container
docker-compose down

# Reset database (delete all data)
docker-compose down -v
```

---

## Troubleshooting

### Port 5432 already in use:
```sh
# Find process using port 5432
netstat -ano | findstr :5432

# Stop the container
docker-compose down
```

### Cannot connect to Docker database:
- Ensure Docker is running
- Check container status: `docker ps`
- Verify connection string in `appsettings.Development.json`

### Cannot connect to Supabase:
- Check your internet connection
- Verify credentials in `appsettings.Supabase.json`
- Ensure Supabase instance is active

---

## Security Notes

- **Never commit sensitive connection strings to Git**
- Use **User Secrets** or **Environment Variables** for production
- The `appsettings.Supabase.json` should ideally be in `.gitignore` if it contains real credentials

---

Last Updated: 2025-01-20
