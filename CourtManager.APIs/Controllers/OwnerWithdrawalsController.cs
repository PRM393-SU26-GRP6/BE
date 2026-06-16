using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Withdrawals.Commands;
using CourtManager.Application.Features.Withdrawals.Queries;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/owner")]
[Authorize(Roles = "Owner")]
public class OwnerWithdrawalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OwnerWithdrawalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get owner's wallet summary.
    /// </summary>
    [HttpGet("wallet")]
    public async Task<ActionResult<OwnerWalletDto>> GetWallet(CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await _mediator.Send(new GetOwnerWalletQuery(ownerId), cancellationToken);
        return Ok(new { success = true, data = result });
    }

    /// <summary>
    /// Get owner's wallet transaction history.
    /// </summary>
    [HttpGet("wallet-history")]
    public async Task<ActionResult<WalletHistoryDto>> GetWalletHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var result = await _mediator.Send(new GetWalletHistoryQuery(ownerId, page, pageSize), cancellationToken);
        return Ok(new { success = true, data = result });
    }

    /// <summary>
    /// Create a withdrawal request.
    /// </summary>
    [HttpPost("withdrawal-requests")]
    public async Task<ActionResult<WithdrawalRequestResultDto>> CreateWithdrawalRequest(
        [FromBody] CreateWithdrawalRequestDto request,
        CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var command = new CreateWithdrawalRequestCommand
        {
            OwnerId = ownerId,
            Amount = request.Amount,
            BankName = request.BankName,
            BankAccountNumber = request.BankAccountNumber,
            BankAccountHolderName = request.BankAccountHolderName
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetWithdrawalRequest), new { id = result.Id }, new { success = true, data = result });
    }

    /// <summary>
    /// Get owner's withdrawal requests.
    /// </summary>
    [HttpGet("withdrawal-requests")]
    public async Task<ActionResult<IEnumerable<WithdrawalRequestDto>>> GetWithdrawalRequests(CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await _mediator.Send(new GetOwnerWithdrawalRequestsQuery(ownerId), cancellationToken);
        return Ok(new { success = true, data = result });
    }

    /// <summary>
    /// Get a specific withdrawal request.
    /// </summary>
    [HttpGet("withdrawal-requests/{id}")]
    public async Task<ActionResult<WithdrawalRequestDto>> GetWithdrawalRequest(Guid id, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await _mediator.Send(new GetOwnerWithdrawalRequestsQuery(ownerId), cancellationToken);
        var request = result.FirstOrDefault(w => w.Id == id);
        
        if (request == null)
            return NotFound(new { success = false, message = "Withdrawal request not found" });
            
        return Ok(new { success = true, data = request });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }
}
