using FluentValidation;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Validator for VerifyOtpCommand.
/// </summary>
public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required")
            .Length(6).WithMessage("OTP must be exactly 6 characters")
            .Matches(@"^\d{6}$").WithMessage("OTP must contain digits only");
    }
}
