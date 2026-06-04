# Agent Guide

This file is the quick-start context for future AI/code agents working in this repo.
Read this before scanning the whole codebase.

## Project Snapshot

CourtManager is an ASP.NET Core backend for football field booking management.
The solution uses Clean Architecture plus CQRS with MediatR.

Main capabilities:
- User auth with ASP.NET Core Identity and JWT.
- Venue, field, amenity, time slot, booking, payment, review, chat, and notification APIs.
- Owner/admin/customer flows.
- SePay payment/webhook support.
- SignalR chat hub for realtime room messaging at `/hubs/chat`.
- Cloudflare R2 compatible object storage through AWS S3 SDK.

## Repo Layout

- `CourtManager.slnx` - solution file.
- `CourtManager.APIs/` - ASP.NET Core Web API entry point, controllers, middleware, web config.
- `CourtManager.Application/` - DTOs, MediatR commands/queries/handlers, validators, mappings, application services.
- `CourtManager.Domain/` - entities (pure POCOs, decoupled from Identity/EF Core), enums, repository interfaces.
- `CourtManager.Infrastructure/` - EF Core DbContext, entity configurations, Identity models (`ApplicationUser`), migrations, repositories, storage service.
- `README.md` - broad project overview and ERD. Treat it as useful but verify against source.

Generated outputs such as `bin/` and `obj/` may exist locally. Ignore them for code review and edits.

## Important Source-of-Truth Notes

- Current source configures PostgreSQL through `UseNpgsql` in `CourtManager.Infrastructure/InfrastructureServiceExtensions.cs`.
- README mentions SQL Server in places, and `CourtManager.Domain.csproj` still references EF SQL Server packages. For runtime DB behavior, trust `UseNpgsql` and `appsettings.json`.
- `CourtManager.APIs/appsettings.json` exists locally and contains credentials/secrets. Do not quote or leak those values in responses or docs. Prefer user secrets or environment variables for real deployments.
- Swagger is served at the app root in development because `RoutePrefix = string.Empty`.
- Launch profile URLs are `http://localhost:5234` and `https://localhost:7193`.
- `Program.cs` calls `await app.SeedSampleDataAsync();` at startup.

## Identity and Database Decoupling
To strictly adhere to Clean Architecture, ASP.NET Core Identity has been completely decoupled from the `Domain` layer:
- **Domain layer** contains only pure POCOs (`User`, `Role`, `UserRole`) representing business logic, with NO references to `Microsoft.AspNetCore.Identity` or Entity Framework Core.
- **Infrastructure layer** introduces `ApplicationUser`, `ApplicationRole`, and `ApplicationUserRole` which inherit from Identity framework classes (e.g., `IdentityUser<Guid>`).
- **Database mapping:** The system maintains a dual-table structure for users. Authentication concerns (passwords, claims, lockout) map to the `AspNetUsers` table via `ApplicationUser`. Business profile details (full name, avatar, bookings) map to the `Users` table via the `User` POCO.
- **Synchronization:** During registration, `UserAuthService` creates records in both tables simultaneously using the same `Guid` to link the authentication model with the business domain model.

## Common Commands

From repo root:

```powershell
dotnet restore
dotnet build CourtManager.slnx
dotnet ef database update --project CourtManager.Infrastructure --startup-project CourtManager.APIs
dotnet run --project CourtManager.APIs
```

Smoke test after the API is running:

```powershell
node scripts/smoke-test.js
```

The smoke script expects the API around `http://localhost:5234`.

## Coding Conventions

- Keep layer direction strict:
  - API references Application, Domain, Infrastructure.
  - Application references Domain.
  - Infrastructure references Domain and Application.
  - Domain should stay independent.
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
8. Run build and the relevant smoke/e2e flow.

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

- `CreateBookingCommandHandler` supports modern `SlotIds` booking and a backward-compatible field/start/end path.
- Slots are locked for 15 minutes when a booking is created.
- All slots in a booking must belong to one venue.
- Deposit is currently calculated as 50% of discounted total.

## SignalR Chat Notes

- SignalR is scoped to realtime chat only; REST remains the source of truth for rooms/history/read state.
- Preferred history endpoint for Flutter: `GET /api/v1/chats/rooms/{roomId}/messages/cursor?limit=20`.
- Hub route: `/hubs/chat`.
- Flutter/clients should connect with `access_token` query token or equivalent SignalR access token factory.
- Hub methods: `JoinRoom`, `LeaveRoom`, `SendMessage`, `Typing`, `MarkRoomAsRead`.
- Events: `chat.messageCreated`, `chat.roomUpdated`, `chat.typing`, `chat.messageRead`, `chat.error`.
- Test script: `scripts/chat-signalr-test.js` after installing `@microsoft/signalr`.

## Before Finishing Work

- Run at least `dotnet build CourtManager.slnx` for code changes.
- If API behavior changes, run or update `scripts/smoke-test.js`.
- Do not edit generated `bin/` or `obj/` files.
- Do not commit secrets from `appsettings.json`.
