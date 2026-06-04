using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CourtManager.Application.Features.Admin;

public record GetAdminUsersQuery : IRequest<IEnumerable<AdminUserDto>>;
public record UpdateAdminUserRoleCommand(Guid UserId, UpdateUserRoleDto Request) : IRequest<UserRoleResultDto>;
public record GetAdminVenuesQuery : IRequest<IEnumerable<VenueDto>>;
public record UpdateAdminVenueStatusCommand(Guid VenueId, UpdateStatusDto Request) : IRequest<StatusResultDto>;
public record BroadcastNotificationCommand(Guid SenderId, BroadcastNotificationDto Request) : IRequest<BroadcastNotificationResultDto>;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, IEnumerable<AdminUserDto>>
{
    private readonly UserManager<User> _userManager;

    public GetAdminUsersQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public Task<IEnumerable<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users
            .OrderBy(u => u.Email)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.PhoneNumber,
                IsActive = u.IsActive,
                LoyaltyPoints = u.LoyaltyPoints
            })
            .ToList();

        return Task.FromResult<IEnumerable<AdminUserDto>>(users);
    }
}

public class UpdateAdminUserRoleCommandHandler : IRequestHandler<UpdateAdminUserRoleCommand, UserRoleResultDto>
{
    private readonly UserManager<User> _userManager;

    public UpdateAdminUserRoleCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserRoleResultDto> Handle(UpdateAdminUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        var role = request.Request.Role.Trim().ToLowerInvariant() switch
        {
            "admin" => "Admin",
            "owner" or "manager" => "Owner",
            "customer" or "player" or "user" => "User",
            "guest" => "Guest",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ValidationException("Invalid role.");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            throw new ValidationException(string.Join("; ", removeResult.Errors.Select(e => e.Description)));
        }

        var addResult = await _userManager.AddToRoleAsync(user, role);
        if (!addResult.Succeeded)
        {
            throw new ValidationException(string.Join("; ", addResult.Errors.Select(e => e.Description)));
        }

        return new UserRoleResultDto { UserId = request.UserId, Role = role };
    }
}

public class GetAdminVenuesQueryHandler : IRequestHandler<GetAdminVenuesQuery, IEnumerable<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetAdminVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueDto>> Handle(GetAdminVenuesQuery request, CancellationToken cancellationToken)
    {
        var venues = await _venueRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<VenueDto>>(venues);
    }
}

public class UpdateAdminVenueStatusCommandHandler : IRequestHandler<UpdateAdminVenueStatusCommand, StatusResultDto>
{
    private readonly IVenueRepository _venueRepository;

    public UpdateAdminVenueStatusCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<StatusResultDto> Handle(UpdateAdminVenueStatusCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue == null)
        {
            throw new NotFoundException(nameof(Venue), request.VenueId);
        }

        venue.IsActive = request.Request.IsActive;
        venue.UpdatedAt = DateTime.UtcNow;
        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        return new StatusResultDto { Id = request.VenueId, IsActive = venue.IsActive };
    }
}

public class BroadcastNotificationCommandHandler : IRequestHandler<BroadcastNotificationCommand, BroadcastNotificationResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly INotificationRepository _notificationRepository;

    public BroadcastNotificationCommandHandler(UserManager<User> userManager, INotificationRepository notificationRepository)
    {
        _userManager = userManager;
        _notificationRepository = notificationRepository;
    }

    public async Task<BroadcastNotificationResultDto> Handle(BroadcastNotificationCommand request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users
            .Where(u => u.IsActive)
            .Select(u => u.Id)
            .ToList();

        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),
            SenderId = request.SenderId,
            Title = request.Request.Title,
            Message = request.Request.Message,
            Type = NotificationType.Broadcast,
            RefId = request.Request.RefId ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            NotificationRecipients = users.Select(userId => new NotificationRecipient
            {
                RecipientId = Guid.NewGuid(),
                UserId = userId
            }).ToList()
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return new BroadcastNotificationResultDto
        {
            NotificationId = notification.NotificationId,
            Recipients = users.Count
        };
    }
}
