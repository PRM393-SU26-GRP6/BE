## Project Snapshot

CourtManager is an ASP.NET Core backend for football field booking management.
The solution uses Clean Architecture plus CQRS with MediatR.

Main capabilities:
- User auth with ASP.NET Core Identity and JWT.
- Venue, field, amenity, time slot, booking, payment, review, chat, and notification APIs.
- Owner/admin/customer flows.
- SePay payment/webhook support.
- SignalR chat hub for realtime room messaging at `/hubs/chat`.
- SignalR notification hub at `/hubs/notifications`.
- Cloudflare R2 compatible object storage through AWS S3 SDK.


## Repo Layout

- `CourtManager.slnx` - solution file.
- `CourtManager.APIs/` - ASP.NET Core Web API entry point, controllers, middleware, web config.
- `CourtManager.Application/` - DTOs, MediatR commands/queries/handlers, validators, mappings, application exceptions, and interfaces/ports.
- `CourtManager.Domain/` - entities, enums, and repository-facing model types. Current `User`, `Role`, and `UserRole` inherit ASP.NET Core Identity types.
- `CourtManager.Infrastructure/` - EF Core DbContext, entity configurations, migrations, repositories, JWT/password services, storage service, and background services.
- `README.md` - broad project overview and ERD. Treat it as useful but verify against source.

Generated outputs such as `bin/` and `obj/` may exist locally. Ignore them for code review and edits.

## Important Source-of-Truth Notes

- Current source configures PostgreSQL through `UseNpgsql` in `CourtManager.Infrastructure/InfrastructureServiceExtensions.cs`.
- README mentions SQL Server in places, and `CourtManager.Domain.csproj` still references EF SQL Server packages. For runtime DB behavior, trust `UseNpgsql` and `appsettings.json`.
- `CourtManager.APIs/appsettings.json` exists locally and contains credentials/secrets. Do not quote or leak those values in responses or docs. Prefer user secrets or environment variables for real deployments.
- Swagger is served at the app root in development because `RoutePrefix = string.Empty`.
- Launch profile URLs are `http://localhost:5234` and `https://localhost:7193`.
- Runtime seed data is configured through EF Core model seeding in `ApplicationDbContext.SeedData(...)`; there is no current startup call to `SeedSampleDataAsync()`.
- Current seeded slots are dated `2026-01-07`; when testing booking flows after that date, create future slots first.

## Identity and Database Decoupling
Current source does not fully decouple ASP.NET Core Identity from the `Domain` layer:
- `CourtManager.Domain.Entities.User` inherits `IdentityUser<Guid>`.
- `CourtManager.Domain.Entities.Role` inherits `IdentityRole<Guid>`.
- `CourtManager.Domain.Entities.UserRole` inherits `IdentityUserRole<Guid>`.
- `ApplicationDbContext` inherits `IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>`.
- Identity tables are renamed to project table names: `Users`, `Roles`, `UserRoles`, `UserClaims`, `UserLogins`, `RoleClaims`, and `UserTokens`.
- There is no separate `ApplicationUser`/`AspNetUsers` dual-table synchronization model in the current codebase.

## Common Commands

From repo root:

```powershell
dotnet restore
dotnet build CourtManager.slnx
dotnet ef database update --project CourtManager.Infrastructure --startup-project CourtManager.APIs
dotnet run --project CourtManager.APIs
```

## Coding Conventions

- Keep layer direction strict:
  - API references Application, Domain, Infrastructure.
  - Application references Domain.
  - Infrastructure references Domain and Application.
  - Domain currently has ASP.NET Core Identity references for `User`, `Role`, and `UserRole`; do not describe it as framework-free unless this is refactored.
- Controllers should be thin. They usually:
  - define route/auth attributes,
  - get `CurrentUserId` when needed,
  - build/send MediatR commands or queries,
  - return HTTP responses.
- Business logic belongs in Application handlers.
- Data access belongs behind Domain repository interfaces and Infrastructure repository implementations.
- Entity persistence config belongs in `CourtManager.Infrastructure/Data/*Configuration.cs`.
- DTO mappings belong in `CourtManager.Application/Mappings/MappingProfile.cs`.
- Use `DateTime.UtcNow` for persisted timestamps.
- Respect nullable reference types.

## Adding A Feature

Typical path:

1. Add or update Domain entity/enum/repository interface if the model or persistence contract changes.
2. Add Application DTOs, command/query records/classes, validators, and handlers under `CourtManager.Application/Features/<Feature>/`.
3. Add repository implementation or query method in Infrastructure if needed.
4. Register any new repository/service in `InfrastructureServiceExtensions` or `ApplicationServiceExtensions`.
5. Add or update API endpoint in the relevant controller.
6. Update AutoMapper profile when mapping entities to DTOs.
7. Add EF migration if schema changes.


## Auth And Authorization

- Most protected controllers use `[Authorize]`.
- `BaseApiController.CurrentUserId` reads `ClaimTypes.NameIdentifier` and throws `UnauthorizedAccessException` if missing/invalid.
- `CurrentUserService` exposes the current user id to handlers through `ICurrentUserService`.
- Role checks are usually done with `User.IsInRole("Admin")` or `User.IsInRole("Owner")`.
- `RequireRoleAttribute` exists but is only a marker attribute unless extra runtime enforcement is added.

## Error Handling

`GlobalExceptionHandlingMiddleware` maps Application exceptions to HTTP responses:

- `NotFoundException` -> 404.
- `ValidationException` -> 400.
- `UnauthorizedAccessException` -> 401.
- `ForbiddenException` -> 403.
- Other exceptions -> 500.

Prefer throwing the existing Application exceptions from handlers instead of building ad hoc error payloads there.

## Payment Notes

- Payment endpoints live in `CourtManager.APIs/Controllers/PaymentsController.cs`.
- SePay webhook endpoint is anonymous but validates `X-API-Key`.
- SePay QR generation uses payment transaction code with `CM` prefix in the transfer description.
- Payment and booking status transitions should be reviewed carefully before changing because slots are locked/booked/unlocked across workflows.

## Booking Notes

- `CreateBookingCommandHandler` creates bookings from `SlotIds`; frontend should not send `UserId`.
- Slots are locked for 15 minutes when a booking is created.
- All slots in a booking must belong to one venue.
- Deposit is currently calculated as 50% of discounted total.

## SignalR Realtime Notes

- SignalR is used for chat and notifications; REST remains the source of truth for rooms/history/read state and notification lists.
- Hub route: `/hubs/chat`.
- Notification hub route: `/hubs/notifications`.
- Flutter/clients should connect with `access_token` query token or equivalent SignalR access token factory.
- Chat hub methods: `JoinRoom`, `LeaveRoom`, `SendMessage`, `MarkRoomAsRead`.
- Chat events: `chat.roomJoined`, `chat.messageCreated`, `chat.roomUpdated`, `chat.messagesRead`, `chat.error`.
- Notification hub methods: `GetUnreadCount`, `MarkNotificationAsRead`, `MarkAllNotificationsAsRead`.
- Notification events: `notification.created`, `notification.read`, `notification.readAll`, `notification.unreadCountChanged`, `notification.error`.
- History endpoint currently exposed: `GET /api/v1/chats/rooms/{roomId}/messages?pageNumber=1&pageSize=20`.

## Before Finishing Work

- Run at least `dotnet build CourtManager.slnx` for code changes.
- If API behavior changes, verify with Swagger
- Do not edit generated `bin/` or `obj/` files.
- Do not commit secrets from `appsettings.json`.
