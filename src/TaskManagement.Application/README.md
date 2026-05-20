# TaskManagement.Application

This project contains the application layer and use-case orchestration.

## Responsibilities

- Implements commands and queries for the system
- Coordinates repositories, unit of work, identity, caching, and other services
- Contains DTOs returned to the API and UI layers
- Applies cross-cutting behaviors such as validation, logging, caching, and idempotency

## Main Areas

- `Common`: abstractions, interfaces, behaviors, exceptions, and pagination models
- `Features\Auth`: login and registration flows
- `Features\Projects`: project commands, queries, and DTOs
- `Features\Tasks`: task commands, queries, and DTOs

## Notes

- This layer depends on the domain model.
- It should contain workflow logic, not persistence details or UI code.
