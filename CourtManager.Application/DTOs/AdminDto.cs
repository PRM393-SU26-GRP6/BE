namespace CourtManager.Application.DTOs;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public int LoyaltyPoints { get; set; }
}

public class UpdateUserRoleDto
{
    public string Role { get; set; } = string.Empty;
}

public class UserRoleResultDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class BroadcastNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RefId { get; set; }
}

public class BroadcastNotificationResultDto
{
    public Guid NotificationId { get; set; }
    public int Recipients { get; set; }
}

