# Bike Store App

Bike Store App is a modern CRUD web application for browsing and managing bicycles. The app demonstrates clean code principles and N-Tier architecture, with secure authentication, database integration, and full Docker containerization for the backend, frontend, and database.


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

## Technologies
- Entity Framework Core 8
- Angular 17
- Bootstrap 5
- PostgreSQL
- Docker

The entire app is containerized for easy deployment:
- PostgreSQL container – database
- API container – .NET 8 backend with EF Core
- Angular container – built and served through Nginx

Run command for build containers:  
`docker compose up --build`  

Swagger documentation is available at:  
http://localhost:5000/swagger/index.html  

Frontend will be available at:  
http://localhost:4200

---
