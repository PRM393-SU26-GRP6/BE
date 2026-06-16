using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Withdrawals.Commands;
using CourtManager.Application.Features.Withdrawals.Queries;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/admin/withdrawal-requests")]
[Authorize(Roles = "Admin")]
public class AdminWithdrawalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminWithdrawalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all withdrawal requests.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WithdrawalRequestDto>>> GetWithdrawalRequests(
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        Domain.Enums.WithdrawalStatus? statusEnum = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<Domain.Enums.WithdrawalStatus>(status, true, out var parsed))
        {
            statusEnum = parsed;
        }

        var result = await _mediator.Send(new GetAdminWithdrawalRequestsQuery(statusEnum), cancellationToken);
        return Ok(new { success = true, data = result });
    }

    /// <summary>
    /// Get a specific withdrawal request.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WithdrawalRequestDto>> GetWithdrawalRequest(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAdminWithdrawalRequestsQuery(), cancellationToken);
        var request = result.FirstOrDefault(w => w.Id == id);

        if (request == null)
            return NotFound(new { success = false, message = "Withdrawal request not found" });

        return Ok(new { success = true, data = request });
    }

    /// <summary>
    /// Approve a withdrawal request.
    /// </summary>
    [HttpPut("{id}/approve")]
    public async Task<ActionResult> ApproveWithdrawalRequest(Guid id, CancellationToken cancellationToken)
    {
        var adminId = GetCurrentUserId();
        var command = new ApproveWithdrawalCommand
        {
            WithdrawalId = id,
            AdminId = adminId
        };

        var success = await _mediator.Send(command, cancellationToken);
        return Ok(new { success = true, message = "Withdrawal request approved successfully" });
    }

    /// <summary>
    /// Reject a withdrawal request.
    /// </summary>
    [HttpPut("{id}/reject")]
    public async Task<ActionResult> RejectWithdrawalRequest(
        Guid id,
        [FromBody] RejectWithdrawalRequestDto request,
        CancellationToken cancellationToken)
    {
        var adminId = GetCurrentUserId();
        var command = new RejectWithdrawalCommand
        {
            WithdrawalId = id,
            AdminId = adminId,
            Reason = request.Reason
        };

        var success = await _mediator.Send(command, cancellationToken);
        return Ok(new { success = true, message = "Withdrawal request rejected" });
    }

    /// <summary>
    /// Get wallet statistics.
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<WalletSummaryDto>> GetWalletStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWalletStatsQuery(), cancellationToken);
        return Ok(new { success = true, data = result });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }
}
