using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Payments;

public record GetPaymentsByBookingQuery(Guid BookingId, Guid UserId, bool IsOwnerOrAdmin) : IRequest<IEnumerable<PaymentDto>>;
public record GetPaymentByIdQuery(Guid PaymentId, Guid UserId, bool IsOwnerOrAdmin) : IRequest<PaymentDto>;
public record GetPaymentByIdPublicQuery(Guid PaymentId) : IRequest<PaymentDto>;
public record GetPaymentHistoryQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<IEnumerable<PaymentDto>>;
public record ProcessDepositPaymentCommand(Guid UserId, ProcessPaymentRequestDto Request) : IRequest<PaymentDto>;
public record ProcessFullPaymentCommand(Guid UserId, bool IsOwner, bool IsAdmin, ProcessPaymentRequestDto Request) : IRequest<PaymentDto>;
public record RefundPaymentCommand(Guid PaymentId, Guid UserId, bool IsOwnerOrAdmin) : IRequest<PaymentDto>;
public record ProcessPaymentGatewayCallbackCommand(PaymentGatewayCallbackDto Callback, string Gateway) : IRequest<PaymentGatewayCallbackResultDto>;
public record ProcessSePayWebhookCommand(
    long WebhookTransactionId,
    string? Gateway,
    string? AccountNumber,
    string? TransferType,
    string Content,
    decimal TransferAmount,
    string? ReferenceCode) : IRequest<PaymentGatewayCallbackResultDto>;

public class GetPaymentsByBookingQueryHandler : IRequestHandler<GetPaymentsByBookingQuery, IEnumerable<PaymentDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByBookingQueryHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsByBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);
        if (!request.IsOwnerOrAdmin && booking.UserId != request.UserId)
            throw new ValidationException("You are not allowed to view payments for this booking.");

        var payments = await _paymentRepository.GetPaymentsByBookingIdAsync(request.BookingId, cancellationToken);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        if (!request.IsOwnerOrAdmin && payment.Booking?.UserId != request.UserId)
            throw new ValidationException("You are not allowed to view this payment.");

        return _mapper.Map<PaymentDto>(payment);
    }
}

public class GetPaymentByIdPublicQueryHandler : IRequestHandler<GetPaymentByIdPublicQuery, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentByIdPublicQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdPublicQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);

        return _mapper.Map<PaymentDto>(payment);
    }
}

public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, IEnumerable<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentHistoryQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetPaymentHistoryForUserAsync(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}

public class ProcessDepositPaymentCommandHandler : IRequestHandler<ProcessDepositPaymentCommand, PaymentDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public ProcessDepositPaymentCommandHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(ProcessDepositPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.Request.BookingId);
        if (booking.UserId != request.UserId)
            throw new ValidationException("Only the booking customer can pay the deposit.");
        if (booking.BookingStatus != BookingStatus.Accepted)
            throw new ValidationException("Deposit can only be paid after the owner accepts the booking.");
        if (booking.Payments.Any(p => p.PaymentType == PaymentType.Deposit && p.PaymentStatus == PaymentStatus.Success))
            throw new ValidationException("Deposit has already been paid.");

        var payment = CreatePayment(booking.Id, booking.DepositAmount > 0 ? booking.DepositAmount : Math.Round(booking.TotalPrice * 0.5m, 2), PaymentType.Deposit, request.Request);
        var isImmediatePayment = request.Request.PaymentMethod == PaymentMethod.Cash;
        if (isImmediatePayment)
        {
            booking.BookingStatus = BookingStatus.Deposited;
        }
        booking.UpdatedAt = DateTime.UtcNow;
        if (isImmediatePayment)
        {
            foreach (var item in booking.BookingItems)
            {
                if (item.Slot != null)
                {
                    item.Slot.SlotStatus = SlotStatus.Booked;
                    item.Slot.LockedUntil = null;
                    item.Slot.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PaymentDto>(payment);
    }

    public static Payment CreatePayment(Guid bookingId, decimal amount, PaymentType type, ProcessPaymentRequestDto request)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Amount = amount,
            PaymentType = type,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = request.PaymentMethod == PaymentMethod.Cash ? PaymentStatus.Success : PaymentStatus.Pending,
            TransactionCode = string.IsNullOrWhiteSpace(request.TransactionCode)
                ? $"{type.ToString().ToUpperInvariant()}-{Guid.NewGuid():N}"
                : request.TransactionCode.Trim(),
            Gateway = request.PaymentMethod.ToString(),
            PaidAt = request.PaymentMethod == PaymentMethod.Cash ? DateTime.UtcNow : null
        };
    }
}

public class ProcessFullPaymentCommandHandler : IRequestHandler<ProcessFullPaymentCommand, PaymentDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProcessFullPaymentCommandHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IUserRepository userRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(ProcessFullPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.Request.BookingId);
        var isBookingCustomer = booking.UserId == request.UserId;
        var isBookingOwner = request.IsOwner && booking.BookingItems.Any(i =>
            i.Slot?.Field?.Venue?.OwnerId == request.UserId);
        if (!isBookingCustomer && !isBookingOwner && !request.IsAdmin)
            throw new ValidationException("Only the booking customer, booking owner, or admin can pay the remaining amount.");
        if (booking.BookingStatus != BookingStatus.Deposited)
            throw new ValidationException("Deposit must be paid before full payment.");
        if (booking.Payments.Any(p => p.PaymentType == PaymentType.Final && p.PaymentStatus == PaymentStatus.Success))
            throw new ValidationException("Remaining amount has already been paid.");

        var paidDeposit = booking.Payments
            .Where(p => p.PaymentType == PaymentType.Deposit && p.PaymentStatus == PaymentStatus.Success)
            .Sum(p => p.Amount);
        var remainingAmount = booking.TotalPrice - paidDeposit;
        if (remainingAmount <= 0)
            throw new ValidationException("No remaining amount is due.");

        var payment = ProcessDepositPaymentCommandHandler.CreatePayment(booking.Id, remainingAmount, PaymentType.Final, request.Request);
        var isImmediatePayment = request.Request.PaymentMethod == PaymentMethod.Cash;
        if (isImmediatePayment)
        {
            booking.BookingStatus = BookingStatus.Completed;
        }
        booking.UpdatedAt = DateTime.UtcNow;

        if (isImmediatePayment)
        {
            var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);
            if (user != null)
            {
                user.LoyaltyPoints += (int)Math.Floor(booking.TotalPrice / 10000m);
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PaymentDto>(payment);
    }
}

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public RefundPaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        if (!request.IsOwnerOrAdmin && payment.Booking?.UserId != request.UserId)
            throw new ValidationException("You are not allowed to refund this payment.");
        if (payment.PaymentStatus != PaymentStatus.Success)
            throw new ValidationException("Only successful payments can be refunded.");

        payment.PaymentStatus = PaymentStatus.Refunded;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PaymentDto>(payment);
    }
}

public class ProcessPaymentGatewayCallbackCommandHandler : IRequestHandler<ProcessPaymentGatewayCallbackCommand, PaymentGatewayCallbackResultDto>
{
    private readonly IPaymentRepository _paymentRepository;

    public ProcessPaymentGatewayCallbackCommandHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentGatewayCallbackResultDto> Handle(ProcessPaymentGatewayCallbackCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Callback.TransactionCode))
        {
            return BadRequest("transactionCode is required");
        }

        var payment = await _paymentRepository.GetByTransactionIdAsync(request.Callback.TransactionCode, cancellationToken);
        if (payment == null)
        {
            return NotFound("Payment transaction was not found");
        }

        payment.PaymentStatus = request.Callback.Success ? PaymentStatus.Success : PaymentStatus.Failed;
        payment.PaidAt = request.Callback.Success ? DateTime.UtcNow : payment.PaidAt;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Ok("Payment callback processed", payment.Id, payment.PaymentStatus.ToString());
    }

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

public class ProcessSePayWebhookCommandHandler : IRequestHandler<ProcessSePayWebhookCommand, PaymentGatewayCallbackResultDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;

    public ProcessSePayWebhookCommandHandler(IPaymentRepository paymentRepository, IUserRepository userRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
    }

    public async Task<PaymentGatewayCallbackResultDto> Handle(ProcessSePayWebhookCommand request, CancellationToken cancellationToken)
    {
        const string sePayGateway = "SePay";

        if (!string.IsNullOrWhiteSpace(request.TransferType) &&
            !request.TransferType.Equals("in", StringComparison.OrdinalIgnoreCase))
        {
            return ProcessPaymentGatewayCallbackCommandHandler.AcceptedFailure("Ignored non-income SePay transaction.");
        }

        var webhookTransactionId = request.WebhookTransactionId > 0 ? request.WebhookTransactionId.ToString() : null;
        if (!string.IsNullOrWhiteSpace(request.ReferenceCode))
        {
            var duplicateByReference = await _paymentRepository.GetByGatewayReferenceAsync(sePayGateway, request.ReferenceCode, cancellationToken);
            if (duplicateByReference != null)
            {
                return ProcessPaymentGatewayCallbackCommandHandler.Ok("Duplicate SePay webhook ignored", duplicateByReference.Id, duplicateByReference.PaymentStatus.ToString());
            }
        }

        if (!string.IsNullOrWhiteSpace(webhookTransactionId))
        {
            var duplicateByTransactionId = await _paymentRepository.GetByGatewayTransactionIdAsync(sePayGateway, webhookTransactionId, cancellationToken);
            if (duplicateByTransactionId != null)
            {
                return ProcessPaymentGatewayCallbackCommandHandler.Ok("Duplicate SePay webhook ignored", duplicateByTransactionId.Id, duplicateByTransactionId.PaymentStatus.ToString());
            }
        }

        var match = System.Text.RegularExpressions.Regex.Match(request.Content, @"CM([\w-]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return ProcessPaymentGatewayCallbackCommandHandler.AcceptedFailure("Noi dung chuyen khoan khong chua ma CM{TransactionCode}.");
        }

        var payment = await _paymentRepository.GetByTransactionIdAsync(match.Groups[1].Value, cancellationToken);
        if (payment == null)
        {
            return ProcessPaymentGatewayCallbackCommandHandler.AcceptedFailure("Khong tim thay don hang tuong ung");
        }

        if (payment.PaymentStatus == PaymentStatus.Success)
        {
            return ProcessPaymentGatewayCallbackCommandHandler.Ok("Payment already processed", payment.Id, payment.PaymentStatus.ToString());
        }

        if (request.TransferAmount != payment.Amount)
        {
            return ProcessPaymentGatewayCallbackCommandHandler.AcceptedFailure($"So tien khong khop. Can {payment.Amount}, nhan duoc {request.TransferAmount}", payment.Id, payment.PaymentStatus.ToString());
        }

        payment.PaymentStatus = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;
        payment.Gateway = sePayGateway;
        payment.GatewayTransactionId = webhookTransactionId;
        payment.GatewayReferenceCode = request.ReferenceCode;
        payment.GatewayAccountNumber = request.AccountNumber;
        payment.GatewayRawContent = request.Content;

        if (payment.Booking != null)
        {
            payment.Booking.UpdatedAt = DateTime.UtcNow;
            if (payment.PaymentType == PaymentType.Deposit)
            {
                payment.Booking.BookingStatus = BookingStatus.Deposited;
                foreach (var item in payment.Booking.BookingItems)
                {
                    if (item.Slot != null)
                    {
                        item.Slot.SlotStatus = SlotStatus.Booked;
                        item.Slot.LockedUntil = null;
                        item.Slot.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
            else if (payment.PaymentType == PaymentType.Final)
            {
                payment.Booking.BookingStatus = BookingStatus.Completed;
                var user = await _userRepository.GetByIdAsync(payment.Booking.UserId, cancellationToken);
                if (user != null)
                {
                    user.LoyaltyPoints += (int)Math.Floor(payment.Booking.TotalPrice / 10000m);
                    await _userRepository.UpdateAsync(user, cancellationToken);
                }
            }
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return ProcessPaymentGatewayCallbackCommandHandler.Ok("Payment confirmed", payment.Id, payment.PaymentStatus.ToString());
    }
}
