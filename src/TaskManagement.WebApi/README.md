# TaskManagement.WebApi

This project exposes the backend through HTTP APIs.

## Responsibilities

- Hosts the ASP.NET Core API
- Configures middleware, controllers, Swagger, authentication, and session handling
- Maps application commands and queries to HTTP endpoints
- Starts master-data seeding during application startup

## Main Areas

- `Controllers\v1`: versioned API endpoints for auth, projects, and tasks
- `Extensions`: API versioning and Swagger setup
- `Middleware`: correlation, exception handling, and session validation
- `Program.cs`: application startup and pipeline configuration

## Notes

- This project is the entry point for the backend.
- It should stay thin and delegate business work to the application layer.
