using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.TimeSlots.Commands;

/// <summary>
/// Handler for DeleteTimeSlotCommand.
/// Performs soft delete on time slot.
/// </summary>
public class DeleteTimeSlotCommandHandler : IRequestHandler<DeleteTimeSlotCommand, bool>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ILogger<DeleteTimeSlotCommandHandler> _logger;

    public DeleteTimeSlotCommandHandler(ITimeSlotRepository timeSlotRepository, ILogger<DeleteTimeSlotCommandHandler> logger)
    {
        _timeSlotRepository = timeSlotRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTimeSlotCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteTimeSlotCommand for SlotId: {SlotId}", request.SlotId);

        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
        {
            _logger.LogWarning("Time slot {SlotId} not found", request.SlotId);
            throw new NotFoundException($"Time slot with ID {request.SlotId} not found");
        }

        await _timeSlotRepository.DeleteAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Time slot {SlotId} deleted successfully (soft delete)", request.SlotId);
        return true;
    }
}
