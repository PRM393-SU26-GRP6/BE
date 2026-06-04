using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using CourtManager.APIs.Services.Realtime;

namespace CourtManager.APIs.Configuration;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration, JwtSettings jwtSettings)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        services.AddEndpointsApiExplorer();
        services.AddHttpClient();
        services.AddSignalR();
        
        services.AddHttpContextAccessor();
        services.AddScoped<CourtManager.Application.Interfaces.ICurrentUserService, CourtManager.APIs.Services.CurrentUserService>();
        services.AddScoped<IRealtimeEventPublisher, RealtimeEventPublisher>();

        // SePay Configuration
        services.Configure<SePaySettings>(configuration.GetSection("SePay"));

        // Swagger Configuration
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Court Manager API",
                Version = "v1.0.0",
                Description = "RESTful API for managing sports court bookings with Clean Architecture and CQRS pattern",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token in the text input below.\n\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            });

            options.AddSecurityDefinition("SePayApiKey", new OpenApiSecurityScheme
            {
                Name = "X-API-Key",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "SePay webhook API key"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
                    new List<string>()
                }
            });
        });

        // JWT Authentication Configuration
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        // CORS Configuration
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Rate Limiting Configuration
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Global Policy: 100 requests per minute per IP (or globally if IP not available)
            options.AddFixedWindowLimiter("GlobalPolicy", policyOptions =>
            {
                policyOptions.PermitLimit = 100;
                policyOptions.Window = TimeSpan.FromMinutes(1);
                policyOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                policyOptions.QueueLimit = 0;
            });

            // Auth Policy: Strict limit for login/register to prevent brute-force attacks (5 req/min)
            options.AddFixedWindowLimiter("AuthPolicy", policyOptions =>
            {
                policyOptions.PermitLimit = 5;
                policyOptions.Window = TimeSpan.FromMinutes(1);
                policyOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                policyOptions.QueueLimit = 0;
            });
        });

        return services;
    }
}
