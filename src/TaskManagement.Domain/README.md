# TaskManagement.Domain

This project contains the core business model for the task management system.

## Responsibilities

- Defines domain entities such as projects and tasks
- Contains value objects, IDs, enums, and domain errors
- Raises domain events for important business actions
- Keeps business rules independent from infrastructure and UI concerns

## Main Areas

- `Common`: base entity types, result/error handling, and domain markers
- `Projects`: project aggregate, statuses, IDs, and events
- `Tasks`: task entity, priorities, statuses, IDs, and events
- `Users`: user identity value objects used by the domain

## Notes

- This project should not depend on infrastructure or presentation layers.
- Business rules should be implemented here first when they are domain-specific.
