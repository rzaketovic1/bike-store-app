# 🚴 Bike Store App

Bike Store App is a modern CRUD web application for browsing and managing bicycles. Built with **ASP.NET Core (.NET 8)**, **Angular**, and **PostgreSQL**, the app demonstrates clean code principles and N-Tier architecture, with secure authentication, database integration with Docker containerization.

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

### CORS Errors

- Ensure frontend runs on `http://localhost:4200`
- Or update CORS policy in `Program.cs`

### Swagger Not Loading

- Ensure API is running on `http://localhost:5000`

## Key Features

This project demonstrates modern web application development using:

- **Authentication & Authorization** – JWT-based login and registration
- **Product Management (CRUD)** – add, edit, delete, and view bicycles
- **Image Uploads** – store and display product images
- **Filtering & Search** – by brand, type, and other attributes
- **Admin Features** – restricted CRUD operations for admins only
- **RESTful API** – data exchange with validated DTOs

## Architecture

The application follows N-Tier architecture and clean code best practices:

- UI (Angular) – user interface with reusable components and Bootstrap styling
- API Controllers (ASP.NET Core) – handle requests from the frontend
- Services Layer – business logic and authorization
- Repository Layer – database access via Entity Framework Core
- DbContext – database connection and model configuration
- Migrations – versioning and database updates
- JWT Authentication – secure user identity management
- Data Transfer Objects (DTOs) – separation of domain models and API layer
- HttpClient – API communication
- Interceptor – automatic JWT token handling
- Bootstrap 5 – responsive design and UI components
- Swagger – interactive API documentation

---
