# TaskManagement.Blazor

This project contains the interactive Blazor frontend for the system.

## Responsibilities

- Provides a Jira-inspired workspace for exploring seeded data
- Handles login, project browsing, and task board interactions
- Calls the Web API through a typed HTTP client
- Presents session information and project/task workflow visually

## Main Areas

- `Components\Pages`: interactive pages such as the main workspace
- `Components\Layout`: navigation and layout shell
- `Models`: API contract models used by the UI
- `Services`: HTTP client integration with the backend API
- `wwwroot`: application styling and static assets

## Notes

- This is the main presentation layer for the user experience.
- Keep UI concerns here and avoid moving backend business logic into the frontend.
