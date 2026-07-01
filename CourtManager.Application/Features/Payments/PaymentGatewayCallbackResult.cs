using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Payments;

internal static class PaymentGatewayCallbackResult
{
    internal static PaymentGatewayCallbackResultDto Ok(string message, Guid? paymentId = null, string? paymentStatus = null)
    {
        return new PaymentGatewayCallbackResultDto
        {
            StatusCode = 200,
            Success = true,
            Message = message,
            PaymentId = paymentId,
            PaymentStatus = paymentStatus
        };
    }

    internal static PaymentGatewayCallbackResultDto AcceptedFailure(string message, Guid? paymentId = null, string? paymentStatus = null)
    {
        return new PaymentGatewayCallbackResultDto
        {
            StatusCode = 200,
            Success = false,
            Message = message,
            PaymentId = paymentId,
            PaymentStatus = paymentStatus
        };
    }

    internal static PaymentGatewayCallbackResultDto BadRequest(string message)
    {
        return new PaymentGatewayCallbackResultDto { StatusCode = 400, Success = false, Message = message };
    }

    internal static PaymentGatewayCallbackResultDto NotFound(string message)
    {
        return new PaymentGatewayCallbackResultDto { StatusCode = 404, Success = false, Message = message };
    }
}
