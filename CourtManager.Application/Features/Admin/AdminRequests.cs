using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Admin;

public record GetAdminUsersQuery : IRequest<IEnumerable<AdminUserDto>>;
public record UpdateAdminUserRoleCommand(Guid UserId, UpdateUserRoleDto Request) : IRequest<UserRoleResultDto>;
public record GetAdminVenuesQuery : IRequest<IEnumerable<VenueDto>>;
public record UpdateAdminVenueStatusCommand(Guid VenueId, UpdateStatusDto Request) : IRequest<StatusResultDto>;
public record BroadcastNotificationCommand(Guid SenderId, BroadcastNotificationDto Request) : IRequest<BroadcastNotificationResultDto>;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, IEnumerable<AdminUserDto>>
{
    private readonly IUserAuthService _userAuthService;

    public GetAdminUsersQueryHandler(IUserAuthService userAuthService)
    {
        _userAuthService = userAuthService;
    }

    public Task<IEnumerable<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userAuthService.Users
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
    private readonly IUserAuthService _userAuthService;

    public UpdateAdminUserRoleCommandHandler(IUserAuthService userAuthService)
    {
        _userAuthService = userAuthService;
    }

    public async Task<UserRoleResultDto> Handle(UpdateAdminUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userAuthService.FindByIdAsync(request.UserId);
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

        var currentRoles = await _userAuthService.GetRolesAsync(user);
        var (removeSucceeded, removeErrors) = await _userAuthService.RemoveFromRolesAsync(user, currentRoles);
        if (!removeSucceeded)
        {
            throw new ValidationException(string.Join("; ", removeErrors));
        }

        var (addSucceeded, addErrors) = await _userAuthService.AddToRoleAsync(user, role);
        if (!addSucceeded)
        {
            throw new ValidationException(string.Join("; ", addErrors));
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
    private readonly IUserAuthService _userAuthService;
    private readonly INotificationRepository _notificationRepository;

    public BroadcastNotificationCommandHandler(IUserAuthService userAuthService, INotificationRepository notificationRepository)
    {
        _userAuthService = userAuthService;
        _notificationRepository = notificationRepository;
    }

    public async Task<BroadcastNotificationResultDto> Handle(BroadcastNotificationCommand request, CancellationToken cancellationToken)
    {
        var users = _userAuthService.Users
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
