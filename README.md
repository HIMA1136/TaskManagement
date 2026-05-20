# Task Management API

This repository contains a small project and task management system built for a Backend .NET Developer technical assessment.

## Tech Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / LocalDB
- JWT Authentication
- Clean Architecture
- MediatR
- FluentValidation
- Swagger

## Solution Structure

- `src/TaskManagement.Domain`: core business entities, value objects, enums, and domain rules
- `src/TaskManagement.Application`: use cases, DTOs, commands, queries, interfaces, and behaviors
- `src/TaskManagement.Infrastructure`: EF Core, Identity, JWT, repositories, services, and seeding
- `src/TaskManagement.WebApi`: HTTP API, middleware, versioning, and Swagger setup
- `src/TaskManagement.Blazor`: optional demo UI client for manually testing the backend

## Architecture Overview

The solution follows Clean Architecture:

- `Domain` contains the business model and should not depend on outer layers.
- `Application` contains use-case orchestration and abstractions.
- `Infrastructure` implements persistence and external concerns.
- `WebApi` is the entry point and exposes the application through HTTP endpoints.

The write side uses CQRS-style commands and query handlers through MediatR. Validation is handled with FluentValidation pipeline behaviors, and exceptions are normalized through global middleware.

## Features

### Authentication

- Register
- Login

### Projects

- Create project
- Get all projects
- Get project by id
- Update project
- Delete project

### Tasks

- Create task
- Update task status
- Get tasks by project
- Delete task

## Prerequisites

- .NET 9 SDK
- SQL Server LocalDB or SQL Server

## Configuration

The default connection string is in:

- [appsettings.json](C:\Users\hp\source\repos\Task Managment\src\TaskManagement.WebApi\appsettings.json)

Default database:

- `TaskManagementDb`

## Running the API

1. Restore packages:

```powershell
dotnet restore
```

2. Apply database migrations:

```powershell
dotnet ef database update --project src/TaskManagement.Infrastructure --startup-project src/TaskManagement.WebApi
```

3. Run the API:

```powershell
dotnet run --project src/TaskManagement.WebApi
```

4. Open Swagger:

- `http://localhost:54491/swagger`

## Seeded Demo User

The application seeds sample data on startup.

- Email: `owner@taskmanagement.local`
- Password: `Pass1234`

## Demo UI

An optional Blazor project is included as a lightweight client for testing the API manually.

Run it with:

```powershell
dotnet run --project src/TaskManagement.Blazor
```

## Database Migrations

Migration files are located under the Infrastructure project in the EF Core migrations folder.

## API Documentation

Swagger is enabled in Development and can be used to test all endpoints.

## Notes

- The backend is the primary deliverable for the assessment.
- The Blazor project is included only as a local testing/demo client.
