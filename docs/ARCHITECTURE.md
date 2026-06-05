# Architecture

CourtManager is a layered ASP.NET Core backend using Clean Architecture and CQRS.
The main dependency flow is inward toward Domain, while runtime composition happens in the API project.

## Solution Structure

```text
CourtManager.APIs
  -> CourtManager.Application
  -> CourtManager.Domain
  -> CourtManager.Infrastructure

CourtManager.Application
  -> CourtManager.Domain

CourtManager.Infrastructure
  -> CourtManager.Domain
  -> CourtManager.Application

CourtManager.Domain
  -> no project references (Pure .NET BCL, zero framework dependencies)
```

## Runtime Startup

`CourtManager.APIs/Program.cs` is the app entry point.

Startup flow:

1. Bind JWT settings from configuration.
2. Register Application services through `AddApplicationServices()`.
3. Register Infrastructure services through `AddInfrastructureServices(configuration)`.
4. Register JWT token service.
5. Register API services through `AddWebApiServices(configuration, jwtSettings)`.
6. Build app.
7. Seed sample data with `SeedSampleDataAsync()`.
8. Add global exception middleware, static files, Swagger in development, routing, rate limiting, CORS, authentication, authorization, and controllers.

## Layer Responsibilities

### API Layer

Project: `CourtManager.APIs`

Responsibilities:
- HTTP routing and controller actions.
- Authentication/authorization attributes and role checks.
- Current-user extraction for request context.
- Swagger/OpenAPI, CORS, rate limiting.
- SignalR hub endpoint for realtime chat.
- Global exception response mapping.
- Web-specific models for external callbacks such as SePay webhooks.

Key files:
- `Program.cs`
- `Configuration/ApiServiceExtensions.cs`
- `Controllers/*Controller.cs`
- `Middleware/GlobalExceptionHandlingMiddleware.cs`
- `Services/CurrentUserService.cs`
- `Hubs/ChatHub.cs`

Controllers generally dispatch to MediatR and should stay thin.

### Application Layer

Project: `CourtManager.Application`

Responsibilities:
- Use cases implemented as MediatR commands and queries.
- DTOs and request/response models used by API/application boundaries.
- FluentValidation validators.
- AutoMapper profiles.
- Application-specific exceptions.
- Interfaces for external concerns needed by use cases, such as current user and storage.
- Auth helper services such as JWT token creation and password hashing.

Key folders:
- `Features/<Feature>/Commands`
- `Features/<Feature>/Queries`
- `DTOs`
- `Exceptions`
- `Interfaces`
- `Mappings`
- `Services`

Main libraries:
- MediatR
- FluentValidation
- AutoMapper
- BCrypt.Net
- System.IdentityModel.Tokens.Jwt

### Domain Layer

Project: `CourtManager.Domain`

Responsibilities:
    - Core entities (Pure POCOs, completely decoupled from Entity Framework and ASP.NET Identity).
- Enums.
- Repository interfaces (Note: Under strict Clean Architecture, these ports should eventually move to the Application layer).
- Domain model shape independent of web and persistence details.

Key folders:
- `Entities`
- `Enums`
- `Interfaces`

Important aggregate areas:
- Users and roles.
- Venues, football fields, venue images, amenities.
- Time slots and bookings.
- Payments and discounts.
- Reviews.
- Chats and messages.
- Notifications and recipients.

### Infrastructure Layer

Project: `CourtManager.Infrastructure`

Responsibilities:
- EF Core `ApplicationDbContext`.
- Entity type configurations.
- Migrations.
- Repository implementations.
- ASP.NET Core Identity EF stores using separate `ApplicationUser` and `ApplicationRole` entities that map to ASP.NET Identity tables (e.g., `AspNetUsers`), allowing Domain `User` and `Role` to remain pure business models.
- Cloudflare R2 storage service via AWS S3 compatible client.

Key files/folders:
- `ApplicationDbContext.cs`
- `InfrastructureServiceExtensions.cs`
- `Data/*Configuration.cs`
- `Data/SampleDataSeeder.cs`
- `Migrations`
- `Repositories`
- `Services/CloudflareR2StorageService.cs`

Runtime database provider:
- PostgreSQL through `Npgsql.EntityFrameworkCore.PostgreSQL`.

## Request Flow

Typical protected API request:

```text
Client
  -> Controller action
  -> BaseApiController.CurrentUserId or ICurrentUserService
  -> MediatR command/query
  -> Application handler
  -> Domain repository interface
  -> Infrastructure repository
  -> ApplicationDbContext / EF Core
  -> AutoMapper DTO
  -> Controller response
```

Exceptions thrown by handlers bubble to `GlobalExceptionHandlingMiddleware`, which maps known Application exceptions to HTTP status codes.

## Dependency Injection

Application registrations:
- MediatR handlers from the Application assembly.
- FluentValidation validators from the Application assembly.
- AutoMapper `MappingProfile`.
- `IPasswordHasherService`.
- JWT token service via `AddJwtTokenService(...)`.

Infrastructure registrations:
- `ApplicationDbContext` with PostgreSQL.
- IdentityCore with `User`, `Role`, and `ApplicationDbContext`.
- AWS S3 compatible client for Cloudflare R2.
- Repository interfaces to concrete repositories.
- `IStorageService` to `CloudflareR2StorageService`.

API registrations:
- Controllers with JSON enum strings and cycle ignoring.
- Swagger with JWT bearer security and SePay API key security definition.
- `ICurrentUserService` to `CurrentUserService`.
- SePay settings binding.
- CORS policy `AllowAll`.
- Fixed-window rate limiters `GlobalPolicy` and `AuthPolicy`.

## Persistence Model

`ApplicationDbContext` inherits:

```text
IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole,
IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
```

This ensures a strict separation between Authentication concerns (`AspNetUsers`) and Business Domain concerns (`Users`).

Domain DbSets include:
- Venues, venue images, amenities, venue amenities.
- Football fields and time slots.
- Bookings, booking items, booking discounts.
- Payments.
- Chat rooms and messages.
- Notifications and notification recipients.
- Reviews.
- User devices.

Entity configuration is split into `CourtManager.Infrastructure/Data`.
Some seed data is configured in `ApplicationDbContext.SeedData`, and broader sample data is seeded at startup by `SampleDataSeeder`.

## Major Workflows

### Authentication

- `AuthController` exposes register, login, refresh-token, and logout.
- Login/register are rate-limited with `AuthPolicy`.
- **Identity Decoupling:** Registration (`UserAuthService.CreateAsync`) writes to two places simultaneously:
  1. `UserManager` creates an `ApplicationUser` in the `AspNetUsers` table to handle authentication concerns (passwords, tokens).
  2. `ApplicationDbContext` creates a pure `User` POCO in the `Users` table using the identical `Guid` to handle business relationships.
- JWT bearer validation uses issuer, audience, signing key, and zero clock skew.
- Current user id is expected in `ClaimTypes.NameIdentifier`.

### Booking

- `BookingsController` requires authorization.
- Create booking forces `command.UserId` to the logged-in user.
- `CreateBookingCommandHandler` validates user and slots, applies discount, creates booking items, locks slots, and notifies the venue owner.
- Slots are considered unavailable unless `SlotStatus` is `Available` or already locked by the same user.

### Payments

- `PaymentsController` requires authorization except callbacks/webhooks.
- Deposit, final payment, refund, history, and booking payment queries are handled through MediatR.
- Generic gateway callback is anonymous.
- SePay webhook validates `X-API-Key` before dispatching.
- SePay QR endpoint returns bank info and QR URL data for a payment.

### Realtime Chat

- REST chat endpoints remain the source of truth for room creation, room list, message history, and read state.
- Message history supports cursor pagination at `GET /api/v1/chats/rooms/{roomId}/messages/cursor`.
- SignalR hub route is `/hubs/chat`.
- JWT Bearer reads `access_token` from the query string only for `/hubs/chat`.
- On connect, `ChatHub` adds the connection to `user:{userId}`.
- `JoinRoom(roomId)` verifies room membership before adding the connection to `chatroom:{roomId}`.
- `SendMessage` calls `SendMessageCommand`, waits for DB save, then emits `chat.messageCreated` and `chat.roomUpdated`.
- `MarkRoomAsRead` calls `MarkRoomAsReadCommand`, then emits `chat.messageRead` and `chat.roomUpdated`.

Recommended Flutter flow:

```text
Open chat screen
  -> GET /messages/cursor?limit=20
  -> JoinRoom(roomId)
  -> append new SignalR chat.messageCreated events
  -> load older history with nextCursor.beforeMessageId when scrolling up
```

### Venue Discovery And Owner Management

- Public venue/field discovery is handled by venue and field controllers/queries.
- Owner-specific management endpoints use owner controllers plus Application feature handlers.
- Venue images use `IStorageService`, currently backed by Cloudflare R2.

## API Surface Map

Controller files currently include:
- `AdminController`
- `AmenitiesController`
- `AuthController`
- `BookingsController`
- `ChatsController`
- `DiscountsController`
- `FieldsController`
- `NotificationsController`
- `OwnerController`
- `OwnerFieldsController`
- `OwnerVenuesController`
- `PaymentsController`
- `ReviewsController`
- `TimeSlotsController`
- `UsersController`
- `VenuesController`

Most routes follow `api/v1/<resource>`.

## Data And Config Cautions

- Do not rely only on README for database provider; source currently uses PostgreSQL.
- Do not expose values from local `appsettings.json`; it may include live-looking secrets.
- Prefer environment variables, user secrets, or deployment secret stores for sensitive config.
- Keep `bin/`, `obj/`, and other generated outputs out of manual edits.

## Known Gaps To Verify Before Larger Changes

- No test project is currently present in the solution.
- Node smoke scripts exist, but `scripts/` is ignored by `.gitignore`.
- Some auth-related controller methods are marked `[NonAction]`, so they are not exposed despite having implementation.
- README seeded account details may differ from current EF seed data; verify against `ApplicationDbContext` and `SampleDataSeeder`.
