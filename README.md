# HomeServer API

HomeServer API is an ASP.NET Core backend for a self-hosted home server/management application. It provides a REST API for user authentication, session handling, aquarium maintenance, vehicle records, calendar events, dashboard summaries, file uploads, and PostgreSQL persistence through Entity Framework Core.

The project follows a lightweight clean-architecture approach, with HTTP endpoints, application services, domain models, and infrastructure concerns separated into dedicated projects. This keeps the codebase modular, easier to test, and ready for further feature development.

## Tech stack

- ASP.NET Core 10 Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer authentication
- Refresh-token based sessions
- BCrypt password hashing
- Swagger / OpenAPI
- In-memory caching
- ASP.NET Core rate limiting
- Docker

## Solution structure

```text
HomeServer.API             HTTP API, controllers, middleware, DI, Swagger, CORS, rate limiting
HomeServer.Application     Services, interfaces, DTOs, validators, use cases, result handling
HomeServer.Domain          Models, enums, shared domain types
HomeServer.Infrastructure  EF Core DbContext, migrations, file storage, entity configuration, security helpers
```

## Features

- User authentication with JWT access tokens and refresh-token based sessions
- Secure session cookies for login, refresh, and logout flows
- Role-based authorization for protected administrator and moderator actions
- User management and profile endpoint for the currently authenticated user
- Aquarium management:
  - create, update, delete, list, details, and edit-data endpoints
  - select-list endpoint for UI dropdowns
  - photo upload, paginated photo listing, and photo deletion
  - related water changes and parameter checks
- Water change tracking:
  - create, update, delete, list, and edit-data endpoints
  - aquarium-specific water change creation and editing
  - paginated and filtered list endpoint
- Water parameter check tracking:
  - create, update, delete, list, and edit-data endpoints
  - aquarium-specific parameter check creation and editing
  - paginated and filtered list endpoint
- Vehicle management:
  - vehicle list, details, edit-data, create, update, and delete endpoints
  - vehicle select-list endpoint for UI forms
  - related fuelings and service history
- Vehicle fueling tracking with global and vehicle-specific flows
- Vehicle service history with service item support
- Calendar endpoint aggregating events from multiple modules
- Custom events with create, update, delete, and mark-as-done flows
- Dashboard endpoints for summaries, upcoming events, recent activity, and attention items
- File storage support for uploaded images
- Protected image download endpoint
- Request DTO validation, including image extension, content type, and file size validation
- Global exception handling with standardized `ProblemDetails` responses
- Typed result handling for application service responses
- Soft-delete support for domain entities based on a shared base model
- Entity Framework Core migrations and PostgreSQL persistence
- Optional automatic database migration on application startup
- CLI command flow for creating users from the terminal
- Dockerfile and Docker Compose configuration for containerized deployment

## API modules

| Module | Responsibility |
| --- | --- |
| `Auth` | Login, refresh session, logout |
| `Users` | Current profile, user list, edit, password update, delete |
| `Aquariums` | Aquariums, details, photos, select lists, related maintenance records |
| `WaterChanges` | Water change records, including aquarium-specific flows |
| `ParameterChecks` | Aquarium water parameter measurements |
| `Vehicles` | Vehicle records, base data, related fuelings and services |
| `VehicleFuelings` | Fueling history and edit flows |
| `VehicleServices` | Service history and service items |
| `Calendar` | Aggregated calendar events |
| `CustomEvents` | User-defined events and completion state |
| `Dashboard` | Summaries, attention items, upcoming events, recent activity |
| `Files` | Protected image access |

## Requirements

- .NET 10 SDK
- PostgreSQL Server
- Optional: Docker Desktop or another Docker-compatible runtime
- Optional: EF Core CLI tools for migration management

Install EF Core CLI tools if needed:

```powershell
dotnet tool install --global dotnet-ef
```

## Configuration

The default PostgreSQL connection string is stored in `HomeServer.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DatabaseConnection": "Host=localhost;Port=5432;Database=home_server_db;Username=postgres;Password=YOUR_PASSWORD"
}
```

JWT signing keys are intentionally not stored in `appsettings.json`. For local development, set them with User Secrets:

```powershell
cd HomeServer.API
dotnet user-secrets set "Jwt:Access:Key" "YOUR_ACCESS_TOKEN_SECRET"
dotnet user-secrets set "Jwt:Refresh:Key" "YOUR_REFRESH_TOKEN_SECRET"
```

You can generate a strong local secret in PowerShell:

```powershell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
```

Run the command twice and use different values for access and refresh tokens.

For production-like environments, prefer environment variables:

```powershell
$env:Jwt__Access__Key="YOUR_ACCESS_TOKEN_SECRET"
$env:Jwt__Refresh__Key="YOUR_REFRESH_TOKEN_SECRET"
```

The repository also includes `.env.schema` for Docker-based configuration:

```env
RUN_MIGRATIONS=

POSTGRES_HOST=
POSTGRES_PORT=5432
POSTGRES_DATABASE=
POSTGRES_USERNAME=
POSTGRES_PASSWORD=

CORS_ORIGIN=

JWT_ACCESS_KEY=
JWT_REFRESH_KEY=
```

Docker Compose maps these values to the application configuration, including:

```text
ConnectionStrings__DatabaseConnection
AllowedOrigins
Jwt__Access__Key
Jwt__Refresh__Key
RUN_MIGRATIONS
```

## Database setup

Apply EF Core migrations from the solution directory:

```powershell
dotnet ef database update --project HomeServer.Infrastructure --startup-project HomeServer.API
```

If you prefer Visual Studio Package Manager Console:

```powershell
Update-Database -Project HomeServer.Infrastructure -StartupProject HomeServer.API
```

The application can also apply migrations automatically on startup when `RUN_MIGRATIONS` is set to `true`.

## CLI commands

### Create a user from the terminal

The API project includes a simple CLI command for creating a user directly in the database. This is useful for initializing the first administrator account.

From the solution directory:

```powershell
dotnet run --project HomeServer.API -- user
```

The command prompts for:

- display name
- username
- email
- password
- role: `User`, `Moderator`, or `Admin`

## Run the API

From the solution directory:

```powershell
dotnet restore
dotnet build
dotnet run --project HomeServer.API
```

Swagger is available in development at:

```text
https://localhost:7085/swagger
http://localhost:5092/swagger
```

## Run with Docker

Create a `.env` file based on `.env.schema`, fill in the PostgreSQL, CORS, and JWT values, then run:

```powershell
docker compose up --build
```

Docker Compose builds the API image from `HomeServer.API/Dockerfile` and injects runtime configuration through environment variables.

## Useful commands

```powershell
dotnet restore
dotnet build
dotnet run --project HomeServer.API
dotnet ef migrations add MigrationName --project HomeServer.Infrastructure --startup-project HomeServer.API
dotnet ef database update --project HomeServer.Infrastructure --startup-project HomeServer.API
dotnet user-secrets list --project HomeServer.API
```

## Security and error handling

HomeServer API uses JWT Bearer authentication for protected endpoints and refresh-token based sessions for session renewal. Passwords are hashed with BCrypt, and selected operations are restricted by user roles.

Application service results are mapped to consistent HTTP responses, while unhandled exceptions are processed by a global exception handler. Validation errors are returned in a predictable format, which makes the API easier to integrate with frontend clients.