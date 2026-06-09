using FluentValidation;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Validator for UpdateProfileCommand.
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^(\+84|0)\d{9,10}$").WithMessage("Phone number must be a valid Vietnamese number (starts with 0 or +84 and has 9-10 digits)");

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("Avatar URL must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }
}
