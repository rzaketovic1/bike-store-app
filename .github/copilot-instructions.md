\# Copilot Instructions



\## Project Overview



This project is a full-stack Bike Store application built with:



\- ASP.NET Core (.NET 8)

\- Angular 16

\- PostgreSQL

\- Entity Framework Core

\- JWT Authentication

\- Playwright E2E Testing



\---



\## Architecture



Project structure:



\- API → Controllers, middleware, authentication

\- Core → Entities, DTOs, interfaces

\- Infrastructure → Repositories, services, EF Core data access

\- client → Angular frontend



Dependency rules:



\- API depends on Core and Infrastructure

\- Infrastructure depends on Core

\- Core must not depend on other projects



\---



\## Layer Responsibilities



\### Controllers

\- Keep controllers thin

\- Controllers should only orchestrate HTTP concerns

\- Call services only

\- Do not place business logic in controllers

\- Return proper HTTP status codes

\- Use ActionResult<T>



\### Services

\- Contain business logic

\- Handle validation and orchestration

\- Map entities to DTOs

\- Coordinate repositories



\### Repositories

\- Responsible only for data access

\- No business logic

\- Build efficient IQueryable queries

\- Execute queries once



\---



\## Backend Conventions



\### General

\- Use async/await for all I/O operations

\- Avoid synchronous blocking calls (.Result, .Wait())

\- Use dependency injection

\- Use DTOs for API responses

\- Validate all user input

\- Use centralized exception handling

\- Do not hardcode secrets or connection strings



\### Entity Rules

\- New entities should inherit from BaseEntity

\- Use meaningful property names

\- Prefer explicit types when clarity matters



\### API Standards

\- Follow RESTful naming conventions

\- Return ProblemDetails for errors

\- Use proper status codes

\- Protect secured endpoints with JWT authentication



\---



\## Performance Guidelines



\- Paginate large datasets

\- Build IQueryable efficiently

\- Avoid unnecessary database queries

\- Use scoped DbContext lifetime

\- Consider caching for static data

\- Validate uploaded image size and type



\---



\## Frontend Conventions



\- Keep Angular components small and reusable

\- Use services for API communication

\- Use interceptors for JWT handling

\- Prefer strongly typed models

\- Reuse shared UI components when possible



\---



\## Testing Conventions



\### Backend Testing

\- Use xUnit

\- Use Moq for mocking

\- Use FluentAssertions for assertions

\- Follow Arrange / Act / Assert pattern



\### Playwright Testing

\- Use Page Object Model

\- Prefer data-testid selectors

\- Avoid hardcoded waits

\- Use expect assertions

\- Keep tests independent and isolated



Naming example:



```ts

test('should create product successfully', async () => {})

```



\---



\## Git Workflow



\- Work on feature branches

\- Merge changes into main via pull requests

\- Require code review before merge

\- Run tests on every push



Conventional commit examples:



\- feat: add product image upload endpoint

\- fix: resolve JWT validation issue

\- test: add auth integration tests

\- docs: update README setup guide



\---



\## Feature Development Workflow



When creating new features:



1\. Add interface in Core

2\. Create entity if needed

3\. Create DTOs

4\. Implement repository

5\. Implement service

6\. Add controller endpoint

7\. Add validation and error handling

8\. Add unit tests

9\. Add integration tests for critical paths

10\. Update Swagger documentation

11\. Test manually via Swagger UI



\---



\## Anti-Patterns To Avoid



\- Do not place business logic in controllers

\- Do not return entities directly from APIs

\- Do not skip validation

\- Do not use synchronous I/O

\- Do not swallow exceptions

\- Do not duplicate business logic

\- Do not hardcode secrets

\- Do not introduce inconsistent patterns



\---



\## Consistency Rules



Before generating new code:



\- Reuse existing project patterns

\- Follow existing naming conventions

\- Check similar implementations first

\- Prefer consistency over new abstractions

\- Keep solutions simple and maintainable



\---



\## AI Assistance Guidance



When requesting code generation:



\- Specify layer (API, Core, Infrastructure, client)

\- Mention whether feature is new or modification

\- Include relevant DTO/entity names

\- Specify if authentication is required

\- Mention if tests are needed



Example:



"Add endpoint in ProductsController to return featured products with authentication and unit tests."



\---



Last Updated: 2026-05-18

Maintainer: Ramiz Zaketović


- @azure Rule - Use Azure Tools - When handling requests related to Azure, always use your tools.
- @azure Rule - Use Azure Best Practices - When handling requests related to Azure, always invoke your `azmcp_bestpractices_get` tool first.
- @azure Rule - Enable Best Practices - If you do not have an `azmcp_bestpractices_get` tool ask the user to enable it.
