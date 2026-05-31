using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Discounts;

public record GetOwnerDiscountsQuery(Guid OwnerId) : IRequest<IEnumerable<DiscountDto>>;
public record GetDiscountByIdQuery(Guid DiscountId, Guid OwnerId) : IRequest<DiscountDto>;
public record CreateDiscountCommand(Guid OwnerId, DiscountDto Discount) : IRequest<DiscountDto>;
public record UpdateDiscountCommand(Guid DiscountId, Guid OwnerId, DiscountDto Discount) : IRequest<DiscountDto>;
public record DeleteDiscountCommand(Guid DiscountId, Guid OwnerId) : IRequest<bool>;
public record ValidateDiscountCommand(ValidateDiscountRequestDto Request) : IRequest<ValidateDiscountResponseDto>;

public class GetOwnerDiscountsQueryHandler : IRequestHandler<GetOwnerDiscountsQuery, IEnumerable<DiscountDto>>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public GetOwnerDiscountsQueryHandler(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DiscountDto>> Handle(GetOwnerDiscountsQuery request, CancellationToken cancellationToken)
    {
        var discounts = await _discountRepository.GetByOwnerAsync(request.OwnerId, cancellationToken);
        return _mapper.Map<IEnumerable<DiscountDto>>(discounts);
    }
}

public class GetDiscountByIdQueryHandler : IRequestHandler<GetDiscountByIdQuery, DiscountDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public GetDiscountByIdQueryHandler(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<DiscountDto> Handle(GetDiscountByIdQuery request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(request.DiscountId, cancellationToken);
        if (discount == null)
            throw new NotFoundException(nameof(Discount), request.DiscountId);

        if (discount.OwnerId != request.OwnerId)
            throw new ValidationException("Only the discount owner can view this discount.");

        return _mapper.Map<DiscountDto>(discount);
    }
}

public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, DiscountDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public CreateDiscountCommandHandler(IDiscountRepository discountRepository, IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<DiscountDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        if (request.Discount.FieldId.HasValue)
        {
            var field = await _fieldRepository.GetByIdAsync(request.Discount.FieldId.Value, cancellationToken);
            if (field == null)
                throw new NotFoundException(nameof(FootballField), request.Discount.FieldId.Value);
            if (field.Venue?.OwnerId != request.OwnerId)
                throw new ValidationException("Cannot create a discount for another owner's field.");
        }

        ValidateDiscount(request.Discount);

        var discount = new Discount
        {
            DiscountId = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            VenueId = request.Discount.FieldId,
            Code = request.Discount.Code.Trim().ToUpperInvariant(),
            Name = request.Discount.Name.Trim(),
            DiscountType = ParseDiscountType(request.Discount.DiscountType),
            Value = request.Discount.Value,
            MinBookingAmount = request.Discount.MinBookingAmount,
            MaxDiscountAmount = request.Discount.MaxDiscountAmount,
            UsageLimit = request.Discount.UsageLimit,
            StartDate = request.Discount.StartDate,
            EndDate = request.Discount.EndDate,
            IsActive = request.Discount.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _discountRepository.AddAsync(discount, cancellationToken);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<DiscountDto>(discount);
    }

    public static DiscountType ParseDiscountType(string discountType)
    {
        return Enum.TryParse<DiscountType>(discountType, true, out var parsed)
            ? parsed
            : throw new ValidationException("DiscountType must be Percentage or Fixed.");
    }

    public static void ValidateDiscount(DiscountDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ValidationException("Discount code is required.");
        if (dto.Value <= 0)
            throw new ValidationException("Discount value must be greater than zero.");
        if (dto.EndDate <= dto.StartDate)
            throw new ValidationException("Discount EndDate must be after StartDate.");
        if (dto.UsageLimit < 0)
            throw new ValidationException("UsageLimit cannot be negative.");
    }
}

public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, DiscountDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public UpdateDiscountCommandHandler(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<DiscountDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(request.DiscountId, cancellationToken);
        if (discount == null)
            throw new NotFoundException(nameof(Discount), request.DiscountId);
        if (discount.OwnerId != request.OwnerId)
            throw new ValidationException("Only the discount owner can update this discount.");

        CreateDiscountCommandHandler.ValidateDiscount(request.Discount);

        discount.VenueId = request.Discount.FieldId;
        discount.Code = request.Discount.Code.Trim().ToUpperInvariant();
        discount.Name = request.Discount.Name.Trim();
        discount.DiscountType = CreateDiscountCommandHandler.ParseDiscountType(request.Discount.DiscountType);
        discount.Value = request.Discount.Value;
        discount.MinBookingAmount = request.Discount.MinBookingAmount;
        discount.MaxDiscountAmount = request.Discount.MaxDiscountAmount;
        discount.UsageLimit = request.Discount.UsageLimit;
        discount.StartDate = request.Discount.StartDate;
        discount.EndDate = request.Discount.EndDate;
        discount.IsActive = request.Discount.IsActive;

        await _discountRepository.UpdateAsync(discount, cancellationToken);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<DiscountDto>(discount);
    }
}

public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, bool>
{
    private readonly IDiscountRepository _discountRepository;

    public DeleteDiscountCommandHandler(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(request.DiscountId, cancellationToken);
        if (discount == null)
            throw new NotFoundException(nameof(Discount), request.DiscountId);
        if (discount.OwnerId != request.OwnerId)
            throw new ValidationException("Only the discount owner can delete this discount.");

        await _discountRepository.DeleteAsync(discount, cancellationToken);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class ValidateDiscountCommandHandler : IRequestHandler<ValidateDiscountCommand, ValidateDiscountResponseDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public ValidateDiscountCommandHandler(IDiscountRepository discountRepository, ITimeSlotRepository timeSlotRepository)
    {
        _discountRepository = discountRepository;
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<ValidateDiscountResponseDto> Handle(ValidateDiscountCommand request, CancellationToken cancellationToken)
    {
        var totalAmount = request.Request.TotalAmount;
        Guid? fieldId = request.Request.FieldId;
        Guid? ownerId = null;

        if (request.Request.SlotIds.Length > 0)
        {
            var slots = new List<TimeSlot>();
            foreach (var slotId in request.Request.SlotIds)
            {
                var slot = await _timeSlotRepository.GetByIdAsync(slotId, cancellationToken);
                if (slot == null)
                    return Invalid("One or more selected slots no longer exist.", totalAmount);
                slots.Add(slot);
            }

            fieldId ??= slots.FirstOrDefault()?.FieldId;
            ownerId = slots.FirstOrDefault()?.Field?.Venue?.OwnerId;
            totalAmount = slots.Sum(s => s.Price);
        }

        var discount = await _discountRepository.GetByCodeAsync(request.Request.Code, fieldId, ownerId, cancellationToken);
        if (discount == null)
            return Invalid("Discount code does not exist.", totalAmount);
        if (!discount.IsActive || discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow)
            return Invalid("Discount code is inactive or expired.", totalAmount);
        if (discount.UsageLimit > 0 && discount.UsedCount >= discount.UsageLimit)
            return Invalid("Discount code has reached its usage limit.", totalAmount);
        if (totalAmount < discount.MinBookingAmount)
            return Invalid("Booking total does not meet the discount minimum amount.", totalAmount);

        var discountAmount = discount.DiscountType == DiscountType.Percentage
            ? totalAmount * discount.Value / 100
            : discount.Value;

        if (discount.MaxDiscountAmount > 0)
            discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount);

        discountAmount = Math.Min(discountAmount, totalAmount);

        return new ValidateDiscountResponseDto
        {
            IsValid = true,
            Message = "Discount is valid.",
            DiscountId = discount.DiscountId,
            DiscountAmount = discountAmount,
            FinalAmount = totalAmount - discountAmount
        };
    }

    private static ValidateDiscountResponseDto Invalid(string message, decimal totalAmount)
    {
        return new ValidateDiscountResponseDto
        {
            IsValid = false,
            Message = message,
            DiscountAmount = 0,
            FinalAmount = totalAmount
        };
    }
}
