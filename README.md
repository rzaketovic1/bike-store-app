# 🚴 Bike Store App

A full-stack e-commerce application for browsing and managing bicycles, showcasing modern web development practices and enterprise-level architecture. Built with **ASP.NET Core (.NET 8)**, **Angular 17**, and **PostgreSQL**, this project demonstrates clean architecture principles, secure JWT authentication, RESTful API design, and containerized deployment with Docker.

**Key Highlights:**
- 🏗️ **N-Tier Architecture** – clean separation of concerns across API, Service, Repository, and Data layers
- 🔐 **JWT Authentication** – secure token-based user authentication and authorization
- 📦 **Entity Framework Core** – code-first migrations and database seeding
- 🎨 **Angular SPA** – responsive UI with Bootstrap 5 and reactive forms
- 🐳 **Docker Support** – containerized PostgreSQL for consistent development environments
- 📸 **File Uploads** – image management with validation and secure storage
- 🔍 **Advanced Filtering** – pagination, sorting, and multi-criteria search
- 📝 **Swagger/OpenAPI** – interactive API documentation and testing

## 📦 Prerequisites

Install the following tools before running the app:

- **.NET 8 SDK**  
  Verify with:
  `dotnet --version`
- **Node.js 18+** and **npm** 
  Verify with:
  ```
  node -v
  npm -v
  ```
- **PostgreSQL 16+**
- **Docker Desktop** *(optional but recommended)*

> ℹ️ Angular CLI is **not required globally**. The project uses local dependencies.

## 🚀 Run Locally

### 1) Start PostgreSQL


```bash
docker compose up -d
```

### 2) Run Backend API

```bash
cd API
dotnet restore
dotnet build
dotnet run
```

API + Swagger:

- Swagger UI: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)
- API Endpoint: [http://localhost:5000/api/Products](http://localhost:5000/api/Products)

> ⚠️ On startup, the API automatically applies EF Core migrations and seeds initial data.

### 3) Run Angular Client

```bash
cd client
npm install
npm start
```

Frontend:

- [http://localhost:4200](http://localhost:4200)

## 🔐 Authentication

The API uses **JWT Bearer Authentication**.

- **Public endpoints** → accessible without authentication
- **Protected endpoints** → require a valid JWT token

### Auth Endpoints

- `POST /api/Auth/register`
- `POST /api/Auth/login`

### Usage

Add the token to the request header:

```
Authorization: Bearer <your_token>
```

## 📡 API Overview

### 🛍️ Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Products` | Paginated list (filters: `brand`, `type`, `sort`, `pageIndex`, `pageSize`) |
| GET | `/api/Products/{id}` | Get product by ID |
| GET | `/api/Products/brands` | List brands |
| GET | `/api/Products/types` | List types |
| POST | `/api/Products` | Create product (auth required) |
| POST | `/api/Products/with-image` | Create product with image (auth required) |
| PUT | `/api/Products/{id}` | Update product (auth required) |
| PUT | `/api/Products/{id}/with-image` | Update product with image (auth required) |
| DELETE | `/api/Products/{id}` | Delete product (auth required) |

## 🖼️ Upload Constraints

- Allowed formats: `jpg`, `jpeg`, `png`, `gif`, `webp`
- Max file size: **5MB**

## 🌐 CORS & Static Files

- Allowed origin: `http://localhost:4200`
- Static files (uploads) are served via `UseStaticFiles()`

## 🏗️ Project Structure

```
API             # ASP.NET Core Web API
Infrastructure  # Data access, repositories, services
Core            # Entities, DTOs, interfaces
client          # Angular frontend
```

## 📝 Notes

- Backend: **.NET 8**
- Frontend: **Angular 17**
- Database migrations and seeding run automatically on startup

## 🛠️ Troubleshooting

### 401 Unauthorized

Check JWT configuration in `API/appsettings.Development.json`:

- `Jwt:Key` (minimum 32 characters)
- `Jwt:Issuer`
- `Jwt:Audience`

### Database Connection Failed

- Ensure PostgreSQL is running
- Verify `ConnectionStrings:DefaultConnection`

### Reset the database to initial state

- Drop the database completely
  `dotnet ef database drop --project Infrastructure --startup-project API`
- Recreate and apply migrations with seed data
  `dotnet ef database update --project Infrastructure --startup-project API`
### CORS Errors

- Ensure frontend runs on `http://localhost:4200`
- Or update CORS policy in `Program.cs`

### Swagger Not Loading

- Ensure API is running on `http://localhost:5000`

---
