using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Chats;

public record GetChatRoomsQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<IEnumerable<ChatRoomDto>>;
public record GetChatRoomForUserQuery(Guid UserId, Guid RoomId) : IRequest<ChatRoomDto>;
public record GetOrCreateChatRoomQuery(Guid UserId, Guid OtherUserId) : IRequest<ChatRoomDto>;
public record GetOrCreateVenueChatRoomQuery(Guid UserId, Guid VenueId) : IRequest<ChatRoomDto>;
public record GetMessagesQuery(Guid UserId, Guid RoomId, int PageNumber, int PageSize) : IRequest<IEnumerable<MessageDto>>;
public record GetMessagesCursorQuery(Guid UserId, Guid RoomId, DateTime? BeforeSentAt, Guid? BeforeMessageId, int Limit) : IRequest<CursorPagedResult<MessageDto>>;
public record MarkRoomAsReadCommand(Guid UserId, Guid RoomId) : IRequest<int>;
public record SendMessageCommand(Guid UserId, Guid RoomId, string MessageText) : IRequest<MessageDto>;

public class GetChatRoomsQueryHandler : IRequestHandler<GetChatRoomsQuery, IEnumerable<ChatRoomDto>>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetChatRoomsQueryHandler(IChatRoomRepository chatRoomRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChatRoomDto>> Handle(GetChatRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _chatRoomRepository.GetChatRoomsForUserPaginatedAsync(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
        var result = new List<ChatRoomDto>();

        foreach (var room in rooms)
        {
            var dto = _mapper.Map<ChatRoomDto>(room);
            var last = await _messageRepository.GetLastMessageAsync(room.RoomId, cancellationToken);
            dto.LastMessagePreview = last?.MessageText;
            dto.LastMessageTime = last?.SentAt ?? room.LastMessageAt;
            dto.UnreadCount = await _messageRepository.GetUnreadCountForRoomAsync(room.RoomId, request.UserId, cancellationToken);
            result.Add(dto);
        }

        return result;
    }
}

public class GetChatRoomForUserQueryHandler : IRequestHandler<GetChatRoomForUserQuery, ChatRoomDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetChatRoomForUserQueryHandler(IChatRoomRepository chatRoomRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<ChatRoomDto> Handle(GetChatRoomForUserQuery request, CancellationToken cancellationToken)
    {
        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new NotFoundException(nameof(ChatRoom), request.RoomId);
        if (room.CustomerId != request.UserId && room.HostId != request.UserId)
            throw new ForbiddenException("You are not a participant in this chat room.");

        var dto = _mapper.Map<ChatRoomDto>(room);
        var last = await _messageRepository.GetLastMessageAsync(room.RoomId, cancellationToken);
        dto.LastMessagePreview = last?.MessageText;
        dto.LastMessageTime = last?.SentAt ?? room.LastMessageAt;
        dto.UnreadCount = await _messageRepository.GetUnreadCountForRoomAsync(room.RoomId, request.UserId, cancellationToken);
        return dto;
    }
}

public class GetOrCreateChatRoomQueryHandler : IRequestHandler<GetOrCreateChatRoomQuery, ChatRoomDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMapper _mapper;

    public GetOrCreateChatRoomQueryHandler(IChatRoomRepository chatRoomRepository, IMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _mapper = mapper;
    }

    public async Task<ChatRoomDto> Handle(GetOrCreateChatRoomQuery request, CancellationToken cancellationToken)
    {
        var room = await _chatRoomRepository.GetOrCreateChatRoomAsync(request.UserId, request.OtherUserId, cancellationToken);
        await _chatRoomRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ChatRoomDto>(room);
    }
}

public class GetOrCreateVenueChatRoomQueryHandler : IRequestHandler<GetOrCreateVenueChatRoomQuery, ChatRoomDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMapper _mapper;

    public GetOrCreateVenueChatRoomQueryHandler(IVenueRepository venueRepository, IChatRoomRepository chatRoomRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _chatRoomRepository = chatRoomRepository;
        _mapper = mapper;
    }

    public async Task<ChatRoomDto> Handle(GetOrCreateVenueChatRoomQuery request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetVenueByIdAsync(request.VenueId, cancellationToken);
        if (venue == null)
            throw new NotFoundException(nameof(Venue), request.VenueId);

        var room = await _chatRoomRepository.GetOrCreateChatRoomAsync(request.UserId, venue.OwnerId, cancellationToken);
        await _chatRoomRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ChatRoomDto>(room);
    }
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessagesQueryHandler(IChatRoomRepository chatRoomRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new NotFoundException(nameof(ChatRoom), request.RoomId);
        if (room.CustomerId != request.UserId && room.HostId != request.UserId)
            throw new ForbiddenException("You are not a participant in this chat room.");

        var messages = await _messageRepository.GetMessagesByRoomIdAsync(request.RoomId, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<MessageDto>>(messages.OrderBy(m => m.SentAt));
    }
}

public class GetMessagesCursorQueryHandler : IRequestHandler<GetMessagesCursorQuery, CursorPagedResult<MessageDto>>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessagesCursorQueryHandler(IChatRoomRepository chatRoomRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<CursorPagedResult<MessageDto>> Handle(GetMessagesCursorQuery request, CancellationToken cancellationToken)
    {
        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new NotFoundException(nameof(ChatRoom), request.RoomId);
        if (room.CustomerId != request.UserId && room.HostId != request.UserId)
            throw new ForbiddenException("You are not a participant in this chat room.");

        var limit = Math.Clamp(request.Limit, 1, 50);
        var messages = (await _messageRepository.GetMessagesBeforeAsync(
                request.RoomId,
                request.BeforeSentAt,
                request.BeforeMessageId,
                limit + 1,
                cancellationToken))
            .ToList();

        var hasMore = messages.Count > limit;
        var pageMessages = messages.Take(limit).ToList();
        var orderedForDisplay = pageMessages.OrderBy(m => m.SentAt).ToList();
        var oldestMessage = pageMessages.LastOrDefault();

        return new CursorPagedResult<MessageDto>
        {
            Items = _mapper.Map<IEnumerable<MessageDto>>(orderedForDisplay),
            Limit = limit,
            HasMore = hasMore,
            NextCursor = hasMore && oldestMessage != null
                ? new MessageCursorDto
                {
                    BeforeSentAt = oldestMessage.SentAt,
                    BeforeMessageId = oldestMessage.MessageId
                }
                : null
        };
    }
}

public class MarkRoomAsReadCommandHandler : IRequestHandler<MarkRoomAsReadCommand, int>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;

    public MarkRoomAsReadCommandHandler(IChatRoomRepository chatRoomRepository, IMessageRepository messageRepository)
    {
        _chatRoomRepository = chatRoomRepository;
        _messageRepository = messageRepository;
    }

    public async Task<int> Handle(MarkRoomAsReadCommand request, CancellationToken cancellationToken)
    {
        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new NotFoundException(nameof(ChatRoom), request.RoomId);
        if (room.CustomerId != request.UserId && room.HostId != request.UserId)
            throw new ForbiddenException("You are not a participant in this chat room.");

        await _messageRepository.MarkRoomMessagesAsReadAsync(request.RoomId, request.UserId, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        return await _messageRepository.GetUnreadCountForRoomAsync(request.RoomId, request.UserId, cancellationToken);
    }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public SendMessageCommandHandler(
        IChatRoomRepository chatRoomRepository,
        IMessageRepository messageRepository,
        INotificationRepository notificationRepository,
        IMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _messageRepository = messageRepository;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MessageText))
            throw new ValidationException("Message text is required.");

        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new NotFoundException(nameof(ChatRoom), request.RoomId);
        if (room.CustomerId != request.UserId && room.HostId != request.UserId)
            throw new ForbiddenException("You are not a participant in this chat room.");

        var message = new Message
        {
            MessageId = Guid.NewGuid(),
            RoomId = request.RoomId,
            SenderId = request.UserId,
            MessageText = request.MessageText.Trim(),
            SentAt = DateTime.UtcNow
        };

        room.LastMessageAt = message.SentAt;
        var recipientId = room.CustomerId == request.UserId ? room.HostId : room.CustomerId;
        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),
            SenderId = request.UserId,
            Title = "New message",
            Message = message.MessageText.Length > 120 ? message.MessageText[..120] : message.MessageText,
            Type = NotificationType.Chat,
            RefId = request.RoomId.ToString(),
            CreatedAt = DateTime.UtcNow,
            NotificationRecipients =
            [
                new NotificationRecipient
                {
                    RecipientId = Guid.NewGuid(),
                    UserId = recipientId
                }
            ]
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        await _chatRoomRepository.UpdateAsync(room, cancellationToken);
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        var loaded = await _messageRepository.GetByIdAsync(message.MessageId, cancellationToken) ?? message;
        return _mapper.Map<MessageDto>(loaded);
    }
}
