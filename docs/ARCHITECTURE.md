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
  -> Microsoft.AspNetCore.Identity.EntityFrameworkCore
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
7. Add global exception middleware, static files, Swagger in development, routing, rate limiting, CORS, authentication, authorization, controllers, and SignalR hubs.

Current `Program.cs` maps:
- REST controllers through `app.MapControllers().RequireRateLimiting("GlobalPolicy")`.
- Chat hub through `app.MapHub<ChatHub>("/hubs/chat")`.
- Notification hub through `app.MapHub<NotificationHub>("/hubs/notifications")`.

Sample data is configured through EF Core model seeding in `ApplicationDbContext.SeedData(...)`; there is no current startup call to `SeedSampleDataAsync()`.

## Layer Responsibilities

### API Layer

Project: `CourtManager.APIs`

Responsibilities:
- HTTP routing and controller actions.
- Authentication/authorization attributes and role checks.
- Current-user extraction for request context.
- Swagger/OpenAPI, CORS, rate limiting.
- SignalR hub endpoints for realtime chat and notifications.
- Global exception response mapping.
- Web-specific models for external callbacks such as SePay webhooks.
- Static assets under `wwwroot`, including the internal API flow tester.

Key files:
- `Program.cs`
- `Configuration/ApiServiceExtensions.cs`
- `Controllers/*Controller.cs`
- `Middleware/GlobalExceptionHandlingMiddleware.cs`
- `Services/CurrentUserService.cs`
- `Hubs/ChatHub.cs`
- `Hubs/NotificationHub.cs`
- `Services/Realtime/RealtimeEventPublisher.cs`
- `Services/Realtime/RealtimeConstants.cs`

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
- Interfaces for auth/realtime helpers such as `IJwtTokenService`, `IPasswordHasherService`, and `IRealtimeEventPublisher`.

Key folders:
- `Features/<Feature>/Commands`
- `Features/<Feature>/Queries`
- `DTOs`
- `Exceptions`
- `Interfaces`
- `Mappings`

Main libraries:
- MediatR
- FluentValidation
- AutoMapper

### Domain Layer

Project: `CourtManager.Domain`

Responsibilities:
- Core entities and enums.
- Current `User`, `Role`, and `UserRole` entities inherit ASP.NET Core Identity base types.
- Enums.
- Domain model shape is independent of controllers/web transport, but not currently independent of ASP.NET Core Identity.

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
- ASP.NET Core Identity EF stores using `Domain.Entities.User`, `Role`, and `UserRole`.
- JWT token service, password hasher service, and slot unlock background service.
- Cloudflare R2 storage service via AWS S3 compatible client.

Key files/folders:
- `ApplicationDbContext.cs`
- `InfrastructureServiceExtensions.cs`
- `Data/*Configuration.cs`
- `Migrations`
- `Repositories`
- `Services/CloudflareR2StorageService.cs`
- `Services/JwtTokenService.cs`
- `Services/PasswordHasherService.cs`
- `Services/SlotUnlockBackgroundService.cs`
- `ApplicationDbContext.SeedData(...)`

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

Infrastructure registrations:
- `ApplicationDbContext` with PostgreSQL.
- IdentityCore with `User`, `Role`, and `ApplicationDbContext`.
- AWS S3 compatible client for Cloudflare R2.
- Repository interfaces to concrete repositories.
- `IStorageService` to `CloudflareR2StorageService`.
- `IPasswordHasherService` to `PasswordHasherService`.
- `IJwtTokenService` through `AddJwtTokenService(...)`.
- `SlotUnlockBackgroundService` as hosted service.

API registrations:
- Controllers with JSON enum strings and cycle ignoring.
- Swagger with JWT bearer security and SePay API key security definition.
- `ICurrentUserService` to `CurrentUserService`.
- `IRealtimeEventPublisher` to `RealtimeEventPublisher`.
- SignalR.
- SePay settings binding.
- CORS policy `AllowAll`.
- Fixed-window rate limiters `GlobalPolicy` and `AuthPolicy`.

## Persistence Model

`ApplicationDbContext` inherits:

```text
IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole,
IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
```

Identity-backed tables are mapped to project table names in `OnModelCreating`: `Users`, `Roles`, `UserRoles`, `UserClaims`, `UserLogins`, `RoleClaims`, and `UserTokens`.

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
Seed data is configured in `ApplicationDbContext.SeedData`.

## Major Workflows

### Authentication

- `AuthController` exposes register, login, refresh-token, and logout.
- Login/register are rate-limited with `AuthPolicy`.
- Registration uses `UserManager<User>` to create a single `User` record in the mapped `Users` table, adds the default `User` role, generates access/refresh JWTs, and stores the refresh token on the user.
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

### Realtime Chat And Notifications

- REST chat endpoints remain the source of truth for room creation, room list, message history, and read state.
- SignalR hub route is `/hubs/chat`.
- Notification hub route is `/hubs/notifications`.
- JWT Bearer reads `access_token` from the query string for paths starting with `/hubs`.
- On connect, `ChatHub` adds the connection to `user:{userId}`.
- `NotificationHub` also adds the connection to `user:{userId}`.
- `JoinRoom(roomId)` verifies room membership by loading the room through `GetChatRoomByIdQuery` before adding the connection to `chat-room:{roomId}`.
- `SendMessage` calls `SendMessageCommand`; application handlers persist the message and publish realtime events through `IRealtimeEventPublisher`.
- `MarkRoomAsRead` calls `MarkRoomAsReadCommand` and publishes `chat.messagesRead`.
- `NotificationHub` exposes `GetUnreadCount`, `MarkNotificationAsRead`, and `MarkAllNotificationsAsRead`.
- Realtime event names are centralized in `RealtimeConstants`.

Recommended Flutter flow:

```text
Open chat screen
  -> GET /api/v1/chats/rooms/{roomId}/messages?pageNumber=1&pageSize=20
  -> JoinRoom(roomId)
  -> append new SignalR chat.messageCreated events
  -> use pageNumber/pageSize for older history
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
- No `scripts/` smoke-test folder is currently present in the workspace; use Swagger or `wwwroot/api-flow-tester.html` for manual API flow checks.
- Some auth-related controller methods are marked `[NonAction]`, so they are not exposed despite having implementation.
- README seeded account details may differ from current EF seed data; verify against `ApplicationDbContext.SeedData(...)`.
- The docs may mention cursor-based chat history from an earlier design; the currently exposed controller uses page/pageSize history.
