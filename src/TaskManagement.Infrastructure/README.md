# TaskManagement.Infrastructure

This project contains the concrete technical implementations used by the application layer.

## Responsibilities

- Configures EF Core, Identity, JWT authentication, and dependency injection
- Implements repositories, unit of work, and application services
- Handles persistence concerns such as auditing, interceptors, and seeding
- Provides session configuration and startup master-data seeding

## Main Areas

- `Identity`: application user, JWT generation, and identity service
- `Persistence`: DbContext, repositories, entity configurations, interceptors, and seeding
- `Services`: current user, cache, audit, correlation, idempotency, and session services
- `Settings`: strongly typed configuration classes

## Notes

- This layer is where framework and storage details should live.
- Business rules should stay in the domain or application layers unless they are infrastructure-specific.
