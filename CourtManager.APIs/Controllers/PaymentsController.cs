using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Payments;
using CourtManager.APIs.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing payments.
/// Provides payment processing, transaction history, and SePay webhook endpoints.
/// </summary>
[ApiController]
[Route("api/v1/payments")]
[Authorize]
public class PaymentsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;
    private readonly SePaySettings _sePaySettings;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger, IOptions<SePaySettings> sePaySettings)
    {
        _mediator = mediator;
        _logger = logger;
        _sePaySettings = sePaySettings.Value;
    }

    /// <summary>
    /// Gets all payments for a specific booking.
    /// </summary>
    /// <param name="bookingId">The booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of payments</returns>
    [HttpGet("booking/{bookingId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentsByBooking(Guid bookingId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching payments for booking {BookingId}", bookingId);
        var isOwnerOrAdmin = User.IsInRole("Admin") || User.IsInRole("Owner");
        var result = await _mediator.Send(new GetPaymentsByBookingQuery(bookingId, CurrentUserId, isOwnerOrAdmin), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific payment by ID.
    /// </summary>
    /// <param name="id">The payment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching payment {PaymentId}", id);
        var isOwnerOrAdmin = User.IsInRole("Admin") || User.IsInRole("Owner");
        var result = await _mediator.Send(new GetPaymentByIdQuery(id, CurrentUserId, isOwnerOrAdmin), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets payment history for the current user.
    /// </summary>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated payment history</returns>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Fetching payment history for user {UserId}", userId);
        var result = await _mediator.Send(new GetPaymentHistoryQuery(userId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Processes a deposit payment for a booking.
    /// Customer pays the deposit amount after the owner accepts the booking.
    /// </summary>
    /// <param name="request">The payment request data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created payment</returns>
    [HttpPost("deposit")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ProcessDepositPayment(
        [FromBody] ProcessPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Processing deposit payment for booking {BookingId} by user {UserId}", request.BookingId, userId);
        var result = await _mediator.Send(new ProcessDepositPaymentCommand(userId, request), cancellationToken);
        _logger.LogInformation("Deposit payment created with ID: {PaymentId}", result.Id);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Processes the final (remaining) payment for a booking.
    /// Can be paid by customer, venue owner, or admin after deposit is confirmed.
    /// </summary>
    /// <param name="request">The payment request data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created payment</returns>
    [HttpPost("final")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ProcessFullPayment(
        [FromBody] ProcessPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var isOwner = User.IsInRole("Owner");
        var isAdmin = User.IsInRole("Admin");
        _logger.LogInformation("Processing full payment for booking {BookingId} by user {UserId}", request.BookingId, userId);
        var result = await _mediator.Send(new ProcessFullPaymentCommand(userId, isOwner, isAdmin, request), cancellationToken);
        _logger.LogInformation("Full payment created with ID: {PaymentId}", result.Id);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Refunds a payment.
    /// </summary>
    /// <param name="id">The payment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refunded payment details</returns>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefundPayment(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refunding payment {PaymentId}", id);
        var isAdmin = User.IsInRole("Admin");
        var isOwner = User.IsInRole("Owner");
        var result = await _mediator.Send(new RefundPaymentCommand(id, CurrentUserId, isOwner, isAdmin), cancellationToken);
        _logger.LogInformation("Payment {PaymentId} refunded successfully", id);
        return Ok(result);
    }

    /// <summary>
    /// SePay webhook endpoint.
    /// Receives automatic bank transfer notifications from SePay.
    /// Secured by API key header validation.
    /// </summary>
    /// <remarks>
    /// Enter the API key directly in the request parameters.
    ///
    /// Recommended:
    /// - `authorization = Apikey sepay_webhook_local_2026`
    ///
    /// Or alternatively:
    /// - `x-api-key = sepay_webhook_local_2026`
    /// </remarks>
    /// <param name="webhook">The SePay webhook payload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Webhook processing result</returns>
    [HttpPost("webhook/sepay")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaymentGatewayCallbackResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SePayWebhook(
        [FromBody] SePayWebhookDto webhook,
        CancellationToken cancellationToken)
    {
        // Validate API key from Authorization or X-API-Key header
        var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
        var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(authorizationHeader))
        {
            const string prefix = "Apikey ";
            if (authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                apiKey = authorizationHeader[prefix.Length..].Trim();
            }
            else
            {
                // Fallback: If user just types the raw key in Swagger's Authorization box
                apiKey = authorizationHeader.Trim();
            }
        }

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey != _sePaySettings.EffectiveWebhookApiKey)
        {
            _logger.LogWarning("SePay webhook rejected: invalid or missing API key");
            return Unauthorized(new { success = false, message = "Invalid API key" });
        }

        // Validate required fields before processing
        if (webhook.Id == 0)
        {
            _logger.LogWarning("SePay webhook rejected: missing webhook Id");
            return BadRequest(new { success = false, message = "Invalid webhook payload" });
        }

        _logger.LogInformation("Received SePay webhook: Id={WebhookId}, Amount={Amount}, Content={Content}",
            webhook.Id, webhook.TransferAmount, webhook.Content);

        var result = await _mediator.Send(new ProcessSePayWebhookCommand(
            webhook.Id,
            webhook.Gateway,
            webhook.AccountNumber,
            webhook.TransferType,
            webhook.Code,
            webhook.Content,
            webhook.TransferAmount,
            webhook.ReferenceCode,
            webhook.TransactionDate), cancellationToken);

        // SePay requires EXACTLY {"success": true} - no extra fields
        if (result.StatusCode is >= 200 and < 300)
        {
            return Ok(new { success = true });
        }

        return StatusCode(result.StatusCode, new { success = false, message = result.Message });
    }

    /// <summary>
    /// Generates SePay QR code information for a pending payment.
    /// Returns bank account details and transfer description for QR generation.
    /// </summary>
    /// <param name="paymentId">The payment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>QR code information</returns>
    [HttpGet("{paymentId:guid}/sepay-qr")]
    [ProducesResponseType(typeof(SePayQrResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSePayQrInfo(Guid paymentId, CancellationToken cancellationToken)
    {
        var isOwnerOrAdmin = User.IsInRole("Admin") || User.IsInRole("Owner");
        var payment = await _mediator.Send(new GetPaymentByIdPublicQuery(paymentId, CurrentUserId, isOwnerOrAdmin), cancellationToken);

        if (payment.PaymentStatus != CourtManager.Domain.Enums.PaymentStatus.Pending.ToString())
        {
            return BadRequest(new { success = false, message = "Cannot generate QR code. This payment is already processed or cancelled." });
        }

        var description = $"CM{payment.TransactionCode}";
        var encodedDescription = Uri.EscapeDataString(description);
        var encodedHolder = Uri.EscapeDataString(_sePaySettings.AccountName);
        var encodedStore = Uri.EscapeDataString(string.IsNullOrWhiteSpace(_sePaySettings.VietQrStoreName)
            ? "Court Manager"
            : _sePaySettings.VietQrStoreName);

        var qrResponse = new SePayQrResponseDto
        {
            PaymentId = payment.Id,
            Amount = payment.Amount,
            Description = description,
            Status = payment.PaymentStatus,
            VietQrUrl = $"https://vietqr.app/img?bank={_sePaySettings.VietQrBankName}&acc={_sePaySettings.AccountNo}&template=standee&holder={encodedHolder}&store={encodedStore}&amount={payment.Amount}&des={encodedDescription}",
            BankInfo = new BankInfoDto
            {
                BankId = _sePaySettings.BankId,
                AccountNo = _sePaySettings.AccountNo,
                AccountName = _sePaySettings.AccountName
            }
        };

        return Ok(qrResponse);
    }
}
