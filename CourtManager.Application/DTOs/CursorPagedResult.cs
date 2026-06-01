namespace CourtManager.Application.DTOs;

public class CursorPagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Limit { get; set; }
    public bool HasMore { get; set; }
    public MessageCursorDto? NextCursor { get; set; }
}

public class MessageCursorDto
{
    public DateTime BeforeSentAt { get; set; }
    public Guid BeforeMessageId { get; set; }
}
