namespace CourtManager.Application.DTOs;

public class OwnerStatsDto
{
    public int TotalVenues { get; set; }
    public int TotalFields { get; set; }
    public int PendingBookings { get; set; }
    public int AcceptedBookings { get; set; }
    public int CompletedBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal DepositRevenue { get; set; }
    public decimal FinalPaymentRevenue { get; set; }
}
