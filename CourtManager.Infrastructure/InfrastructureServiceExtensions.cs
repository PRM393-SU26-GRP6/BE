using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using CourtManager.Infrastructure.Repositories;
using CourtManager.Application.Interfaces;
using CourtManager.Infrastructure.Services;

namespace CourtManager.Infrastructure;

/// <summary>
/// Extension methods for configuring Infrastructure layer services.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Adds Infrastructure layer services to the DI container.
    /// Configures DbContext and registers repositories.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("CourtManager.Infrastructure")));

        // Register Identity
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Register Cloudflare R2 / AWS S3
        var r2Config = configuration.GetSection("CloudflareR2");
        var awsOptions = new Amazon.S3.AmazonS3Config
        {
            ServiceURL = r2Config["ServiceURL"],
        };

        services.AddSingleton<Amazon.S3.IAmazonS3>(new Amazon.S3.AmazonS3Client(
            r2Config["AccessKey"],
            r2Config["SecretKey"],
            awsOptions
        ));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFootballFieldRepository, FootballFieldRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        services.AddScoped<IVenueImageRepository, VenueImageRepository>();
        services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IAmenityRepository, AmenityRepository>();
        services.AddScoped<IStorageService, CloudflareR2StorageService>();

        return services;
    }
}
