namespace CourtManager.APIs.Services.Realtime;

public static class RealtimeConstants
{
    public static class Events
    {
        public const string ChatRoomJoined = "chat.roomJoined";
        public const string ChatMessageCreated = "chat.messageCreated";
        public const string ChatRoomUpdated = "chat.roomUpdated";
        public const string ChatMessagesRead = "chat.messagesRead";
        public const string ChatTypingStarted = "chat.typingStarted";
        public const string ChatTypingStopped = "chat.typingStopped";
        public const string ChatError = "chat.error";

        public const string NotificationCreated = "notification.created";
        public const string NotificationRead = "notification.read";
        public const string NotificationReadAll = "notification.readAll";
        public const string NotificationUnreadCountChanged = "notification.unreadCountChanged";
        public const string NotificationError = "notification.error";
    }

    public static class Groups
    {
        public static string ChatRoom(Guid roomId) => $"chat-room:{roomId}";
        public static string User(Guid userId) => $"user:{userId}";
    }
}
