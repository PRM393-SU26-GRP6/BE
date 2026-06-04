using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.Chats.Commands;

/// <summary>
/// Handler for DeleteChatRoomCommand.
/// Performs soft delete on chat room.
/// </summary>
public class DeleteChatRoomCommandHandler : IRequestHandler<DeleteChatRoomCommand, bool>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly ILogger<DeleteChatRoomCommandHandler> _logger;

    public DeleteChatRoomCommandHandler(IChatRoomRepository chatRoomRepository, ILogger<DeleteChatRoomCommandHandler> logger)
    {
        _chatRoomRepository = chatRoomRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteChatRoomCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteChatRoomCommand for RoomId: {RoomId}", request.RoomId);

        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
        {
            _logger.LogWarning("Chat room {RoomId} not found", request.RoomId);
            throw new NotFoundException($"Chat room with ID {request.RoomId} not found");
        }

        await _chatRoomRepository.DeleteAsync(room, cancellationToken);
        await _chatRoomRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Chat room {RoomId} deleted successfully (soft delete)", request.RoomId);
        return true;
    }
}
