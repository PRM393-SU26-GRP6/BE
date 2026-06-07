using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.Notifications.Commands;

/// <summary>
/// Handler for DeleteNotificationCommand.
/// Performs soft delete on notification.
/// </summary>
public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<DeleteNotificationCommandHandler> _logger;

    public DeleteNotificationCommandHandler(INotificationRepository notificationRepository, ILogger<DeleteNotificationCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteNotificationCommand for NotificationId: {NotificationId}", request.NotificationId);

        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null)
        {
            _logger.LogWarning("Notification {NotificationId} not found", request.NotificationId);
            throw new NotFoundException($"Notification with ID {request.NotificationId} not found");
        }

        await _notificationRepository.DeleteAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Notification {NotificationId} deleted successfully (soft delete)", request.NotificationId);
        return true;
    }
}
