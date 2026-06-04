# Architecture Overview

* **Architectural style**: Clean Architecture (Onion Architecture), combined with CQRS.
* **Design principles**: Separation of Concerns, Dependency Inversion, Single Responsibility Principle.
* **Layer responsibilities**:
  * **Domain**: Core business entities, enums, and repository abstractions. Completely framework-agnostic (apart from basic .NET).
  * **Application**: Use cases (CQRS), validation, DTOs, mappers. Depends only on the Domain layer.
  * **Infrastructure**: Database access (EF Core), external APIs (AWS S3/Cloudflare R2), authentication implementation (Identity), concrete repositories.
  * **Presentation (APIs)**: Entry point, HTTP routing, controllers, middlewares, SignalR hubs, DI configuration.

# Solution Structure

| Project | Responsibility |
| --- | --- |
| CourtManager.Domain | Core business logic, Entities, Enums, Interfaces. |
| CourtManager.Application | Use case handlers (CQRS), Validation, DTOs, Mappings. |
| CourtManager.Infrastructure | Data access, Identity framework, External Services. |
| CourtManager.APIs | Web API endpoints, Middlewares, SignalR, Startup config. |

# Dependency Diagram

```text
CourtManager.APIs
    ↓     ↓
    ↓   CourtManager.Infrastructure
    ↓     ↓
CourtManager.Application
    ↓
CourtManager.Domain
```

# Layer Responsibilities

* **Domain**: Defines all POCO entities like `Venue`, `User`, `Booking`, etc. Declares `I*Repository` interfaces.
* **Application**: Implements CQRS with MediatR. Features are organized by domain concept (e.g., `Bookings`, `Venues`). Handles auto-mapping between Entities and DTOs.
* **Infrastructure**: Implements `ApplicationDbContext`. Defines Identity configurations using `ApplicationUser` and `ApplicationRole`. Implements Domain's repository interfaces. Houses AWS S3 / Cloudflare R2 integration.
* **Presentation**: Asp.Net Core APIs. Contains Controllers that inject MediatR to send Commands/Queries. Defines GlobalExceptionHandlingMiddleware and SignalR Hubs (`ChatHub`, `NotificationHub`).

# Folder Structure

* `CourtManager.Domain/Entities`: Business models.
* `CourtManager.Application/Features`: CQRS Commands, Queries, and Handlers.
* `CourtManager.Infrastructure/Data`: Entity Framework configurations (IEntityTypeConfiguration).
* `CourtManager.Infrastructure/Repositories`: Implementations of repository interfaces.
* `CourtManager.APIs/Controllers`: REST API controllers mapping to CQRS actions.
* `CourtManager.APIs/Hubs`: SignalR hubs for real-time web socket communication.

# Request Flow

1. HTTP Request arrives at API Controller.
2. GlobalExceptionHandlingMiddleware intercepts request for error handling.
3. Controller constructs a MediatR Command or Query and sends it via `IMediator`.
4. FluentValidation intercepts the Command (if validation behavior is configured).
5. The corresponding Handler in the Application layer executes business logic.
6. Handler calls Repository interface methods (Domain layer abstractions).
7. Infrastructure layer executes SQL queries against PostgreSQL.
8. Data is returned, AutoMapper converts Entities to DTOs in the Application layer.
9. Controller returns JSON response.

# Data Flow

* **Read flow**: Controller -> MediatR Query -> Application Handler -> Repository -> DbContext -> PostgreSQL -> Repository -> Application Handler (mapped to DTO) -> Controller -> JSON.
* **Write flow**: Controller -> MediatR Command -> Validation -> Application Handler -> Repository -> DbContext.SaveChanges -> PostgreSQL.
* **Transaction flow**: Implicitly handled by EF Core `SaveChanges`.
* **Event flow**: Real-time notifications and messages are sent using `IHubContext` (SignalR) from Application/Infrastructure services or controllers.

# Dependency Rules

* **Allowed**:
  * APIs -> Application, Infrastructure
  * Infrastructure -> Application, Domain
  * Application -> Domain
* **Forbidden**:
  * Domain -> Application, Infrastructure, APIs
  * Application -> Infrastructure, APIs
  * Infrastructure -> APIs

# Persistence Architecture

* **Database**: PostgreSQL.
* **DbContext**: `ApplicationDbContext` located in Infrastructure layer. Uses Fluent API configurations.
* **Configurations**: Specific entity configurations are defined in `CourtManager.Infrastructure/Data` (e.g., `VenueConfiguration`, `BookingConfiguration`) applying `IEntityTypeConfiguration<T>`.
* **Migrations**: EF Core Code-First migrations applied from the `CourtManager.Infrastructure` assembly.
* **Repository pattern**: Specific repository abstractions (e.g., `IVenueRepository`) implemented in Infrastructure (`VenueRepository`).

# Security Architecture

* **Authentication**: JWT (JSON Web Tokens). Handled by `JwtTokenService` in Infrastructure. Validated by standard ASP.NET Core JwtBearer middleware.
* **Authorization**: Role-based access control (Admin, Owner, User). Implemented using ASP.NET Core Identity.
* **Permission model**:
  * The system maintains `ApplicationUser` for EF Identity management while keeping a separate `User` entity for business logic.

# External Integrations

* **Storage**: Cloudflare R2 / AWS S3 (`AWSSDK.S3`) for object storage (images).
* **Payments**: SePay, VNPay, MoMo (identified as supported via PaymentMethod and Gateway configurations in seed data).

# Cross-Cutting Concerns

* **Logging**: Built-in ASP.NET Core ILogger.
* **Validation**: FluentValidation in Application layer.
* **Exception handling**: `GlobalExceptionHandlingMiddleware` in APIs layer intercepts unhandled exceptions to return consistent HTTP responses.
* **Caching**: Not explicitly visible in core analysis, likely relies on standard EF Core caching or external cache if implemented later.
* **Observability**: Logging to Console/Debug.
* **Background processing**: `SlotUnlockBackgroundService` (Hosted Service) running in Infrastructure to periodically unlock unpaid time slots.

# Extension Points

* **MediatR Pipeline Behaviors**: Additional cross-cutting behaviors (like centralized logging or caching) can be added to MediatR.
* **Middlewares**: Custom HTTP processing can be added into the pipeline in `Program.cs`.

# Architecture Decisions

* Separation of `ApplicationUser` (Identity) and `User` (Domain): Prevents leaking Identity framework concerns into the core Domain business logic.
* Use of CQRS with MediatR: Prevents "fat controllers" and centralizes business use cases, making them easier to test and modify independently.
* Real-time notifications: Chosen SignalR to push state changes to connected clients immediately.
