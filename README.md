# CRM.API

A production-ready **Contact Relationship Management REST API** built with **.NET 8** and **Clean Architecture** — demonstrating CQRS, JWT authentication, FluentValidation, and containerised deployment with Docker.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Domain Model](#domain-model)
- [API Endpoints](#api-endpoints)
- [Key Implementation Details](#key-implementation-details)
- [Getting Started](#getting-started)
- [Default Credentials](#default-credentials)
- [Running Tests](#running-tests)
- [Future Improvements](#future-improvements)
- [About](#about)
- [License](#license)

---

## Tech Stack

| Category | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core 8 |
| Language | C# 12 |
| Architecture | Clean Architecture + CQRS |
| Mediator | MediatR 12 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2019 |
| Validation | FluentValidation 11 |
| Authentication | JWT Bearer (HS256) |
| Logging | Serilog |
| API Docs | Swagger / Swashbuckle |
| Containerisation | Docker + Docker Compose |
| Testing | xUnit + Moq + FluentAssertions |

---

## Architecture

The solution follows Clean Architecture with strict dependency rules — outer layers depend on inner layers, never the reverse.

```
┌─────────────────────────────────────────────────────────────┐
│                        CRM.API                              │
│   Controllers  │  Auth (JWT)  │  Middleware  │  Program.cs  │
└────────────────────────────┬────────────────────────────────┘
                             │ references
┌────────────────────────────▼────────────────────────────────┐
│                    CRM.Application                          │
│   Features (Commands/Queries)  │  DTOs  │  Validators       │
│   MediatR Handlers  │  Behaviours (Validation Pipeline)     │
└────────────────────────────┬────────────────────────────────┘
                             │ references
┌────────────────────────────▼────────────────────────────────┐
│                    CRM.Infrastructure                       │
│   ApplicationDbContext  │  Repository<T>  │  UnitOfWork     │
│   EF Core Configurations  │  Migrations  │  SeedData        │
└────────────────────────────┬────────────────────────────────┘
                             │ references
┌────────────────────────────▼────────────────────────────────┐
│                      CRM.Domain                             │
│   Entities  │  Interfaces  │  Enums  │  Common (BaseEntity) │
└─────────────────────────────────────────────────────────────┘
```

---

## Domain Model

```
Company ──< Contact >── Opportunity ──< Activity
   │                        │
   └────────────────────────┘
```

| Entity | Key Fields |
|---|---|
| **Company** | Name, Industry, Size, Website |
| **Contact** | FirstName, LastName, Email, Phone, Notes, CompanyId |
| **Opportunity** | Title, Amount, Stage (enum), Probability, ExpectedCloseDate, ContactId, CompanyId |
| **Activity** | Type (enum), Subject, Description, ActivityDate, ContactId, OpportunityId |

All entities inherit from `BaseEntity` which provides `Id (Guid)`, `CreatedAt`, `UpdatedAt`, and soft-delete via `IsDeleted`.

**Enums:**
- `PipelineStage`: Lead, Qualified, Proposal, Negotiation, ClosedWon, ClosedLost
- `ActivityType`: Call, Email, Meeting

---

## API Endpoints

### Authentication

| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register a new user (Admin / Sales / Viewer) |
| POST | `/api/auth/login` | Public | Login and receive a JWT token |

### Contacts

| Method | Endpoint | Roles | Description |
|---|---|---|---|
| GET | `/api/contacts` | Admin, Sales, Viewer | Paginated list with search & sort |
| GET | `/api/contacts/{id}` | Admin, Sales, Viewer | Get contact by ID |
| POST | `/api/contacts` | Admin, Sales | Create contact |
| PUT | `/api/contacts/{id}` | Admin, Sales | Update contact |
| DELETE | `/api/contacts/{id}` | Admin | Delete contact |

### Companies

| Method | Endpoint | Roles | Description |
|---|---|---|---|
| GET | `/api/companies` | Admin, Sales, Viewer | Paginated list with search & sort |
| GET | `/api/companies/{id}` | Admin, Sales, Viewer | Get company by ID |
| POST | `/api/companies` | Admin, Sales | Create company |
| PUT | `/api/companies/{id}` | Admin, Sales | Update company |
| DELETE | `/api/companies/{id}` | Admin | Delete company |

### Opportunities

| Method | Endpoint | Roles | Description |
|---|---|---|---|
| GET | `/api/opportunities` | Admin, Sales, Viewer | Paginated list with search & sort |
| GET | `/api/opportunities/{id}` | Admin, Sales, Viewer | Get opportunity by ID |
| POST | `/api/opportunities` | Admin, Sales | Create opportunity |
| PUT | `/api/opportunities/{id}` | Admin, Sales | Update opportunity |
| DELETE | `/api/opportunities/{id}` | Admin | Delete opportunity |

### Activities

| Method | Endpoint | Roles | Description |
|---|---|---|---|
| GET | `/api/activities` | Admin, Sales, Viewer | Paginated list with search & sort |
| GET | `/api/activities/{id}` | Admin, Sales, Viewer | Get activity by ID |
| POST | `/api/activities` | Admin, Sales | Create activity |
| PUT | `/api/activities/{id}` | Admin, Sales | Update activity |
| DELETE | `/api/activities/{id}` | Admin | Delete activity |

### Paginated list query parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | int | 1 | Page number |
| `pageSize` | int | 10 | Items per page |
| `sortBy` | string | — | Field to sort by |
| `sortOrder` | string | asc | `asc` or `desc` |
| `search` | string | — | Full-text search term |

---

## Key Implementation Details

### CQRS with MediatR
Every operation is a Command (write) or Query (read) dispatched through MediatR. Handlers are self-contained and live alongside their feature in `CRM.Application/Features/{Entity}/Commands|Queries/`.

### Validation Pipeline
A MediatR `IPipelineBehavior<TRequest, TResponse>` intercepts every request before it reaches the handler. FluentValidation validators for each command run automatically — invalid requests throw a `ValidationException` that is caught by the global exception handler and returned as a structured `400 Bad Request`.

### Global Exception Handling
`ExceptionHandlingMiddleware` catches all unhandled exceptions at the pipeline level and returns RFC 7807 Problem Details responses with consistent structure:
```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 400,
  "errors": { "Email": ["A valid email is required."] }
}
```

### JWT Authentication & Role-Based Authorisation
Tokens are signed with HMAC-SHA256 and carry `sub`, `email`, and `role` claims. Three roles are enforced at the action level:

| Role | Permissions |
|---|---|
| **Admin** | Full access — Read, Create, Update, Delete |
| **Sales** | Read + Create + Update |
| **Viewer** | Read only |

Passwords are stored as PBKDF2-SHA256 hashes (100,000 iterations, 16-byte salt) — no third-party identity library required.

### Repository & Unit of Work Pattern
`Repository<T>` provides a generic async data-access layer over EF Core. `IUnitOfWork` wraps `SaveChangesAsync` to ensure atomic commits per command handler. Both are registered with scoped lifetime.

### Soft Deletes
`IsDeleted` on `BaseEntity` combined with a global EF Core query filter on each entity ensures deleted records are excluded from all queries automatically without changing handler code.

---

## Getting Started

### Option 1 — Docker Compose (recommended)

Requires Docker Desktop or Docker Engine with Compose v2.

```bash
git clone https://github.com/your-username/CRM.API.git
cd CRM.API
docker compose up --build
```

The API will be available at `http://localhost:8080`.
Swagger UI: `http://localhost:8080/swagger`

The `api` service waits for SQL Server to pass its health check before starting. EF Core migrations and seed data run automatically on first boot.

### Option 2 — Local Development

**Prerequisites:** .NET 8 SDK, SQL Server 2019+ (or Docker for SQL Server only)

```bash
# Start SQL Server only
docker compose up sqlserver -d

# Apply migrations
dotnet ef database update -p src/CRM.Infrastructure -s src/CRM.API

# Run the API
dotnet run --project src/CRM.API
```

Swagger UI: `https://localhost:5001/swagger`

### Authenticating in Swagger

1. Open Swagger UI and call `POST /api/auth/login` with one of the credentials below.
2. Copy the `token` from the response.
3. Click the **Authorize** button (top right), enter `Bearer <token>`, and click **Authorize**.

---

## Default Credentials

Seed data is inserted automatically on first startup when the database is empty.

| Role | Email | Password |
|---|---|---|
| Admin | `admin@crm.demo` | `Admin123!` |
| Sales | `sales@crm.demo` | `Sales123!` |
| Viewer | `viewer@crm.demo` | `Viewer123!` |

The seed also creates **5 sample companies**, **10 contacts**, **5 opportunities**, and **10 activities** to explore the API immediately.

---

## Running Tests

```bash
# Run all unit tests
dotnet test tests/CRM.UnitTests/

# Run with detailed output
dotnet test tests/CRM.UnitTests/ --logger "console;verbosity=detailed"
```

The unit test suite covers:

| Class | Tests |
|---|---|
| `CreateContactCommandHandler` | Successful creation, correct field mapping, repository and UoW call verification |
| `CreateContactCommandValidator` | Valid input, empty first/last name, empty/invalid/too-long email, too-long phone |
| `GetContactByIdQueryHandler` | Returns mapped DTO, returns null for missing ID, maps company name, call verification |

---

## Future Improvements

- [ ] Refresh token support (sliding JWT expiry)
- [ ] Integration test suite with `WebApplicationFactory` and testcontainers
- [ ] EF Core `UpdatedAt` auto-stamp via `SaveChanges` interceptor
- [ ] `PATCH` endpoints for partial updates
- [ ] Rate limiting middleware
- [ ] Response caching for read-heavy queries
- [ ] OpenTelemetry tracing
- [ ] CI/CD pipeline (GitHub Actions — build, test, push image)
- [ ] Health check endpoint (`/health`) with DB connectivity probe

---

## About

**Built by Pedro** — Senior Software Developer with 20+ years of experience building enterprise applications. Specialized in .NET, API development, Vue.js, and BI solutions.

This project showcases production patterns I apply daily: clean layering, testability by design, consistent error contracts, and secure-by-default authentication — all in a codebase that is easy to extend and reason about.

> Available for freelance projects. Feel free to reach out if you'd like to work together.

---

## License

[MIT](https://opensource.org/licenses/MIT)
