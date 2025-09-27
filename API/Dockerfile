# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj projekte
COPY ./API ./API
COPY ./Core ./Core
COPY ./Infrastructure ./Infrastructure

# Restore + publish
WORKDIR /src/API
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Expose port
EXPOSE 5000

# Start app
ENTRYPOINT ["dotnet", "API.dll"]