# HomeServer API

HomeServer API is an ASP.NET Core backend for a self-hosted home server application. It handles user authentication, JWT access tokens, refresh-token sessions, user profile data, and SQL Server persistence through Entity Framework Core.

The project is structured as a small clean-architecture style solution, with API, application logic, domain models, and infrastructure kept in separate projects.

## Tech stack

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server / LocalDB
- JWT Bearer authentication
- BCrypt password hashing
- Swagger / OpenAPI
- In-memory caching

## Solution structure

```text
HomeServer.API             HTTP API, controllers, middleware, DI, Swagger
HomeServer.Application     Services, Interfaces, Dtos, use cases, result handling
HomeServer.Domain          Models, enums, shared domain types
HomeServer.Infrastructure  EF Core DbContext, migrations, file storage, entity configuration, security helpers
```

## Features

- Aquariums:
	- Create,
	- Get all,
	- Get Photo of aquarium,
	- Photo Upload,
	- Update,
	- Delete
- 

## Requirements

- .NET 8 SDK
- SQL Server LocalDB or another SQL Server instance

## Configuration

The default connection string is stored in `HomeServer.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DatabaseConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HomeServer;Integrated security=true;TrustServerCertificate=True;"
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

## Database setup

Apply EF Core migrations from the solution directory:

```powershell
dotnet ef database update --project HomeServer.Infrastructure --startup-project HomeServer.API
```

If you prefer Visual Studio Package Manager Console:

```powershell
Update-Database -Project HomeServer.Infrastructure -StartupProject HomeServer.API
```

## CLI Commands

### Create User using CLI command

Be sure that you are in API directory:
```powershell
cd .\HomeServer.API\
```

Then start CLI command feature:
```powershell
dotnet dotnet run -- user
```

After this command, fields will appear for you to enter your data.


## Run the API

From the solution directory:

```powershell
dotnet run --project HomeServer.API
```

Swagger is available in development at:

```text
https://localhost:7085/swagger
http://localhost:5092/swagger
```

## Useful commands

```powershell
dotnet restore
dotnet build
dotnet ef migrations add MigrationName --project HomeServer.Infrastructure --startup-project HomeServer.API
dotnet ef database update --project HomeServer.Infrastructure --startup-project HomeServer.API
dotnet user-secrets list --project HomeServer.API
```
