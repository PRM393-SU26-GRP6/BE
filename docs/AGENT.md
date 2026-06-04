# Project Overview

* **Purpose**: Court Manager Backend API to manage sports venue bookings, football fields, discounts, payments, notifications, and real-time chat between customers and venue owners.
* **Core features**:
  * User authentication and authorization (Role-based: Admin, Owner, Customer/User).
  * Venue and Football Field management for owners.
  * Time slot booking system and discount application.
  * Payment processing integration (SePay, VNPay, MoMo supported via PaymentMethod enum).
  * Real-time notifications and chat functionality.
  * Review and rating system for venues.
* **Major modules**:
  * `CourtManager.APIs`: Presentation layer containing controllers, middlewares, SignalR hubs, and application startup configuration.
  * `CourtManager.Application`: Business logic layer containing CQRS patterns (MediatR), FluentValidation rules, AutoMapper profiles, DTOs, and Interfaces.
  * `CourtManager.Domain`: Core domain layer with entities (Venue, Booking, etc.), enums, and repository interfaces.
  * `CourtManager.Infrastructure`: Data access and external services layer containing EF Core configurations, Identity framework, migrations, concrete repositories, and services like AWS S3/Cloudflare R2 and JWT token services.

# Technology Stack

* **Runtime**: .NET 10.0
* **Framework**: ASP.NET Core
* **Database**: PostgreSQL
* **ORM**: Entity Framework Core 10.0.8 (Npgsql)
* **Authentication**: JWT Bearer Tokens, Microsoft ASP.NET Core Identity (IdentityUser, IdentityRole with EF Core stores).
* **Authorization**: Role-based access control.
* **Search**: Built-in EF Core queries.
* **Cache**: Not explicitly implemented in infrastructure yet, potentially MemoryCache or external if added.
* **Messaging**: SignalR (ChatHub, NotificationHub) for real-time web sockets communication.
* **Storage**: Cloudflare R2 / AWS S3 (via AWSSDK.S3) for images (VenueImage, etc.).
* **Testing**: Not observed in the main analysis.

# Development Conventions

* **Naming conventions**: PascalCase for classes, records, properties. camelCase for local variables and parameters.
* **Folder conventions**:
  * APIs: Controllers, Hubs, Middleware, Configuration.
  * Application: Features (grouped by domain like Admin, Auth, Bookings, etc. for CQRS), DTOs, Exceptions, Interfaces, Mappings.
  * Infrastructure: Data (EF configs), Identity, Migrations, Repositories, Services.
* **Service conventions**: Services are interface-based (`IService`), injected via DI in `ApplicationServiceExtensions` and `InfrastructureServiceExtensions`.
* **Validation approach**: `FluentValidation` registered automatically from the Application assembly.
* **Logging approach**: Standard ASP.NET Core logging (configured to Console and Debug in `Program.cs`).
* **Exception handling approach**: Centralized using `GlobalExceptionHandlingMiddleware`.
* **Dependency injection patterns**: Extension methods per project (`AddApplicationServices`, `AddInfrastructureServices`) called in `Program.cs` to keep the startup clean. Repositories registered as Scoped. Background services as HostedService.

# Business Rules

* Users belong to roles (Admin, Owner, User/Customer) which dictactes system privileges.
* Bookings require a deposit or full payment.
* Payment options include Cash, VNPay, MoMo, and SePay.
* Real-time communication requires valid chat rooms between Customer and Host (Owner).
* `SlotUnlockBackgroundService` unlocks pending slots if payment times out.
* Bookings can have discounts applied (Percentage or Fixed).

# Development Workflow

How to:
* **Add a feature**: Create a new folder inside `CourtManager.Application/Features` mapping to the new domain context.
* **Add an endpoint**: Add a new API controller in `CourtManager.APIs/Controllers` inheriting from `BaseApiController`. Inject `IMediator` and map HTTP methods to MediatR commands/queries.
* **Add a use case**: Add a Command/Query record, a Handler class, and a Validator (if needed) in the corresponding `Features` subfolder in the Application layer.
* **Add a service**: Define interface in `CourtManager.Application/Interfaces`, implement in `CourtManager.Infrastructure/Services`, and register in `InfrastructureServiceExtensions.cs`.
* **Add a repository**: Define interface in `CourtManager.Domain/Interfaces`, implement in `CourtManager.Infrastructure/Repositories`, and register as Scoped in `InfrastructureServiceExtensions.cs`.

# Important Constraints

* Architectural restrictions: Clean Architecture strictly applied. Domain must not depend on Application, Application must not depend on Infrastructure. APIs depend on Application and Infrastructure.
* Common pitfalls: Using Identity (`ApplicationUser`) directly in the Domain layer. The project separates `ApplicationUser` (Infrastructure) from `User` (Domain).

# Quick Navigation

* Startup: `CourtManager.APIs/Program.cs`
* Database Context: `CourtManager.Infrastructure/ApplicationDbContext.cs`
* Entity Configurations: `CourtManager.Infrastructure/Data`
* CQRS Handlers: `CourtManager.Application/Features`
* API Controllers: `CourtManager.APIs/Controllers`
