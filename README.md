# Bike Store App

**Bike Store App** is a CRUD web application for browsing and managing bicycles. 
Built with **ASP.NET Core** and **Angular**, the app includes secure user authentication, product CRUD operations, image uploads, and a structured backend N-Tier architecture following clean code principles.  
The backend is built with **.NET 8 (C#)**, the frontend with **Angular**, and data is stored in **PostgreSQL**.  
The app is containerized with Docker.


## 📦 Overview

This project demonstrates modern web application development using:

- 🔐 **JWT-based authentication** (register/login)
- 🛒 **Product management** with image uploads (admin only)
- 📃 **Filtering & listing** of bicycles by brand and type
- 💾 **SQL Server + EF Core** for persistent storage
- 🌐 **RESTful API** with data validation and DTOs
- 📸 **Static file hosting** for product images

## 🚀 Technologies
- [.NET 8](https://dotnet.microsoft.com/) – Web API (C#)
- [Angular](https://angular.io/) – Frontend
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) – ORM
- [PostgreSQL](https://www.postgresql.org/) or [SQLite](https://www.sqlite.org/)
- [Docker](https://www.docker.com/)


Run command for build containers:  
`docker compose up --build`  

Swagger documentation is available at:  
http://localhost:5000/swagger/index.html  

Frontend will be available at:  
http://localhost:4200

---
