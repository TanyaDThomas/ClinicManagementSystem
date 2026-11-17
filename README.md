
# Clinic Management System

A clinic management application built with ASP.NET Core, C#, PostgreSQL, Entity Framework Core, and Angular.

## Architecture

The backend will follow Clean Architecture:

- **Domain** - Entities and domain rules.
- **Application** - DTOs, interfaces, and application services.
- **Infrastructure** - Entity Framework Core, database access, and external integrations.
- **API** - Controllers, authentication, middleware, and dependency injection.

## Technology Stack

- C# / ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Angular (frontend)

## Project Status

Currently in the database design and planning phase.

## Security

- No passwords, API keys, connection strings containing credentials, or other secrets should be committed.
- Development secrets will be stored using ASP.NET Core User Secrets.
- Only fictional patient data will be used in demonstrations.
