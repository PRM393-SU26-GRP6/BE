using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using CourtManager.Application.Mappings;
using CourtManager.Application.Services;

namespace CourtManager.Application;

/// <summary>
/// Extension methods for configuring Application layer services.
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Adds Application layer services to the DI container.
    /// Configures MediatR, FluentValidation, AutoMapper, and Authentication services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR with all handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register AutoMapper
        services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

        // Register Authentication Services
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();

        return services;
    }

    /// <summary>
    /// Adds JWT token service with configuration.
    /// </summary>
    public static IServiceCollection AddJwtTokenService(
        this IServiceCollection services,
        string secret,
        string issuer,
        string audience,
        int accessTokenExpirationInMinutes = 60,
        int refreshTokenExpirationInDays = 7)
    {
        services.AddScoped<IJwtTokenService>(provider =>
            new JwtTokenService(secret, issuer, audience, accessTokenExpirationInMinutes, refreshTokenExpirationInDays));

        return services;
    }
}
