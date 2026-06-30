using CourtManager.Application;
using CourtManager.Infrastructure;
using CourtManager.APIs.Configuration;
using CourtManager.APIs.Middleware;
using CourtManager.APIs.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURATION SECTION
// ============================================================================

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

// ============================================================================
// DEPENDENCY INJECTION REGISTRATION
// ============================================================================

// 1. Application Layer (MediatR, AutoMapper, FluentValidation)
builder.Services.AddApplicationServices();

// 2. Infrastructure Layer (DbContext, Repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. JWT Token Service
builder.Services.AddJwtTokenService(
    jwtSettings.Secret,
    jwtSettings.Issuer,
    jwtSettings.Audience,
    jwtSettings.AccessTokenExpirationInMinutes,
    jwtSettings.RefreshTokenExpirationInDays);

// 4. Web API Layer (Controllers, Swagger, Auth Config, CORS)
builder.Services.AddWebApiServices(builder.Configuration, jwtSettings);

// 5. Background Services
builder.Services.AddHostedService<CourtManager.APIs.Services.OtpCleanupBackgroundService>();

// ============================================================================
// LOGGING CONFIGURATION
// ============================================================================

builder.Logging.ClearProviders().AddConsole().AddDebug();

// ============================================================================
// BUILD APPLICATION
// ============================================================================

var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE CONFIGURATION
// ============================================================================

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Enable Static Files (Required for Custom Swagger CSS)
app.UseStaticFiles();

// Swagger UI Configuration (Enabled for all environments for project demo)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Court Manager API v1.0.0");
    options.RoutePrefix = string.Empty;

    // Inject Custom Cyberpunk Swagger Theme
    options.InjectStylesheet("/swagger-cyberpunk.css");
});

// app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseRouting();

// Enable Rate Limiter middleware
app.UseRateLimiter();

app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("GlobalPolicy");
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");


// ============================================================================
// APPLICATION STARTUP
// ============================================================================

try
{
    app.Logger.LogInformation("Starting Court Manager API with Cyberpunk Swagger Theme...");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application terminated unexpectedly.");
    throw;
}
