using CourtManager.Application;
using CourtManager.Infrastructure;
using CourtManager.APIs.Configuration;
using CourtManager.APIs.Middleware;
using CourtManager.Infrastructure.Data;

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

// ============================================================================
// LOGGING CONFIGURATION
// ============================================================================

builder.Logging.ClearProviders().AddConsole().AddDebug();

// ============================================================================
// BUILD APPLICATION
// ============================================================================

var app = builder.Build();

await app.SeedSampleDataAsync();

// ============================================================================
// MIDDLEWARE PIPELINE CONFIGURATION
// ============================================================================

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Enable Static Files (Required for Custom Swagger CSS)
app.UseStaticFiles();

// Swagger UI Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Court Manager API v1.0.0");
        options.RoutePrefix = string.Empty; 
        
        // Inject Custom Cyberpunk Swagger Theme
        options.InjectStylesheet("/swagger-cyberpunk.css"); 
    });
}

// app.UseHttpsRedirection();
app.UseRouting();

// Enable Rate Limiter middleware
app.UseRateLimiter();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("GlobalPolicy");


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
