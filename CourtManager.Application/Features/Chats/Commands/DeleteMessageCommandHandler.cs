using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.Chats.Commands;

/// <summary>
/// Handler for DeleteMessageCommand.
/// Performs soft delete on message.
/// </summary>
public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<DeleteMessageCommandHandler> _logger;

    public DeleteMessageCommandHandler(IMessageRepository messageRepository, ILogger<DeleteMessageCommandHandler> logger)
    {
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteMessageCommand for MessageId: {MessageId}", request.MessageId);

        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} not found", request.MessageId);
            throw new NotFoundException($"Message with ID {request.MessageId} not found");
        }

        await _messageRepository.DeleteAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Message {MessageId} deleted successfully (soft delete)", request.MessageId);
        return true;
    }
}
