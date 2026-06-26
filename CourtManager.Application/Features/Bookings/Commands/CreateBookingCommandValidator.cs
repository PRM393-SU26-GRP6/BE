using FluentValidation;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Validator for CreateBookingCommand.
/// Validates business rules and data constraints.
/// </summary>
public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.SlotIds)
            .NotEmpty()
            .WithMessage("At least one slot must be selected.")
            .Must(slots => slots.All(id => id != Guid.Empty))
            .WithMessage("Invalid slot ID detected. Please refresh and try again.");
    }
}
