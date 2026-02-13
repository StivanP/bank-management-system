using FluentValidation;

namespace API.DTOs.RequestDTOs.Auth
{
    public class AuthTokenValidator : AbstractValidator<AuthTokenRequest>
    {
        public AuthTokenValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
                .EmailAddress().WithMessage("Email is not in a valid format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(255).WithMessage("Password must not exceed 255 characters.");
        }
    }
}
