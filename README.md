# EnterpriseApi

`EnterpriseApi` is an ASP.NET Core Web API providing CRUD operations for employees and departments. It uses Entity Framework Core Code First with SQL Server and a simple layered architecture.

## Technology

- .NET 10
- ASP.NET Core controllers
- Entity Framework Core 10 with SQL Server
- Swagger/OpenAPI through Swashbuckle
- ASP.NET Core ProblemDetails
- Data annotation validation

The API does not use authentication, authorization, ASP.NET Core Identity, JWT, roles, MediatR, CQRS, or repository and Unit of Work abstractions.

## Solution structure

```text
EnterpriseApi.slnx
├── EnterpriseApi.Api             API host, controllers, Swagger, and error handling
├── EnterpriseApi.Application     DTOs, request models, service contracts, and exceptions
├── EnterpriseApi.Domain          Employee and Department entities
└── EnterpriseApi.Infrastructure  EF Core, SQL Server, migrations, and service implementations
```

Project dependencies are:

```text
EnterpriseApi.Api ──────────────► EnterpriseApi.Application
        │
        └────────────────────────► EnterpriseApi.Infrastructure
                                      │
                                      ├──► EnterpriseApi.Application
                                      └──► EnterpriseApi.Domain

EnterpriseApi.Application ──────► EnterpriseApi.Domain
```

## Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server Express/LocalDB
- EF Core CLI tools

Check the installed versions:

```powershell
dotnet --version
dotnet ef --version
```

If the EF Core tool is missing, install the version matching the project:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.9
```

## Database connection

The API reads the SQL Server connection string named `DefaultConnection` from `EnterpriseApi.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EnterpriseApiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Change `localhost` to your SQL Server instance when necessary. For deployment, provide the connection string through configuration or the `ConnectionStrings__DefaultConnection` environment variable instead of committing credentials.

## Restore and build

Run these commands from the solution directory:

```powershell
dotnet restore EnterpriseApi.slnx
dotnet build EnterpriseApi.slnx --no-restore
```

## Entity Framework migrations

An initial migration is included under `EnterpriseApi.Infrastructure/Persistence/Migrations`.

Create a future migration:

```powershell
dotnet ef migrations add MigrationName `
  --project EnterpriseApi.Infrastructure `
  --startup-project EnterpriseApi.Api `
  --output-dir Persistence/Migrations
```

Apply migrations to the configured database:

```powershell
dotnet ef database update `
  --project EnterpriseApi.Infrastructure `
  --startup-project EnterpriseApi.Api
```

## Run the API

```powershell
dotnet run --project EnterpriseApi.Api
```

In the Development environment, Swagger is available at:

- `https://localhost:7068/swagger`
- `http://localhost:5237/swagger`

## API endpoints

### Departments

| Method | Route | Successful response |
| --- | --- | --- |
| GET | `/api/departments` | `200 OK` |
| GET | `/api/departments/{id}` | `200 OK` |
| POST | `/api/departments` | `201 Created` |
| PUT | `/api/departments/{id}` | `204 No Content` |
| DELETE | `/api/departments/{id}` | `204 No Content` |

### Employees

| Method | Route | Successful response |
| --- | --- | --- |
| GET | `/api/employees` | `200 OK` |
| GET | `/api/employees/{id}` | `200 OK` |
| POST | `/api/employees` | `201 Created` |
| PUT | `/api/employees/{id}` | `204 No Content` |
| DELETE | `/api/employees/{id}` | `204 No Content` |

Common error responses use ProblemDetails:

- `400 Bad Request` for invalid input or an invalid department ID
- `404 Not Found` when a requested resource does not exist
- `409 Conflict` for duplicate unique values or deleting a department that has employees
- `500 Internal Server Error` for unexpected failures, without exposing stack traces

## Test Department CRUD in Swagger

Create a department with `POST /api/departments`:

```json
{
  "name": "Engineering",
  "description": "Software engineering department",
  "isActive": true
}
```

Copy the returned `id`, then test the department GET, PUT, and DELETE operations. A department cannot be deleted until all of its employees have been deleted or moved.

Example update body:

```json
{
  "name": "Product Engineering",
  "description": "Product development and engineering",
  "isActive": true
}
```

## Test Employee CRUD in Swagger

Create a department first, then use its ID with `POST /api/employees`:

```json
{
  "employeeCode": "EMP-001",
  "firstName": "Aarav",
  "lastName": "Sharma",
  "email": "aarav.sharma@example.com",
  "salary": 75000.00,
  "departmentId": 1,
  "isActive": true
}
```

Copy the returned employee `id`, then test the employee GET, PUT, and DELETE operations.

Example update body:

```json
{
  "employeeCode": "EMP-001",
  "firstName": "Aarav",
  "lastName": "Sharma",
  "email": "aarav.sharma@example.com",
  "salary": 80000.00,
  "departmentId": 1,
  "isActive": true
}
```

`EmployeeCode`, employee email, and department name are unique.
