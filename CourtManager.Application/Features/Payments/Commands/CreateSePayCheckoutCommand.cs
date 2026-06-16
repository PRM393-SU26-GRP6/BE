using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace CourtManager.Application.Features.Payments.Commands;

public class CreateSePayCheckoutCommand : IRequest<SePayCheckoutFormDto>
{
    public Guid PaymentId { get; set; }
    public string MerchantId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string CheckoutBaseUrl { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string ErrorUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;

    public CreateSePayCheckoutCommand(
        Guid paymentId,
        string merchantId,
        string secretKey,
        string checkoutBaseUrl,
        string successUrl,
        string errorUrl,
        string cancelUrl)
    {
        PaymentId = paymentId;
        MerchantId = merchantId;
        SecretKey = secretKey;
        CheckoutBaseUrl = checkoutBaseUrl;
        SuccessUrl = successUrl;
        ErrorUrl = errorUrl;
        CancelUrl = cancelUrl;
    }
}

public class CreateSePayCheckoutCommandHandler : IRequestHandler<CreateSePayCheckoutCommand, SePayCheckoutFormDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<CreateSePayCheckoutCommandHandler> _logger;

    public CreateSePayCheckoutCommandHandler(IPaymentRepository paymentRepository, ILogger<CreateSePayCheckoutCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<SePayCheckoutFormDto> Handle(CreateSePayCheckoutCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {request.PaymentId} not found");
        }

        // Build form data
        var dto = new SePayCheckoutFormDto
        {
            PayUrl = $"{request.CheckoutBaseUrl.TrimEnd('/')}/v1/checkout/init",
            Merchant = request.MerchantId,
            Operation = "PURCHASE",
            PaymentMethod = "BANK_TRANSFER", // Default to bank transfer QR payment
            OrderAmount = Math.Round(payment.Amount).ToString(),
            Currency = "VND",
            OrderInvoiceNumber = payment.TransactionCode,
            OrderDescription = $"CM{payment.TransactionCode}",
            CustomerId = payment.Booking?.UserId.ToString() ?? "",
            SuccessUrl = request.SuccessUrl,
            ErrorUrl = request.ErrorUrl,
            CancelUrl = request.CancelUrl
        };

        // Create signature string using the exact order specified by SePay:
        // merchant, operation, payment_method, order_amount, currency, order_invoice_number, order_description, customer_id, success_url, error_url, cancel_url
        var fields = new List<string>
        {
            $"merchant={dto.Merchant}",
            $"operation={dto.Operation}",
            $"payment_method={dto.PaymentMethod}",
            $"order_amount={dto.OrderAmount}",
            $"currency={dto.Currency}",
            $"order_invoice_number={dto.OrderInvoiceNumber}",
            $"order_description={dto.OrderDescription}",
            $"customer_id={dto.CustomerId}",
            $"success_url={dto.SuccessUrl}",
            $"error_url={dto.ErrorUrl}",
            $"cancel_url={dto.CancelUrl}"
        };

        string signingString = string.Join(",", fields);

        // Compute HMAC-SHA256 signature
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(request.SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingString));
        dto.Signature = Convert.ToBase64String(hash);

        _logger.LogInformation("Generated SePay checkout payload for payment {PaymentId}. Signature: {Signature}", request.PaymentId, dto.Signature);

        return dto;
    }
}
