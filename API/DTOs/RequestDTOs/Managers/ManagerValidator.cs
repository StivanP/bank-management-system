using FluentValidation;

namespace API.DTOs.RequestDTOs.Managers
{
    public class ManagerValidator : AbstractValidator<ManagerRequest>
    {
        public ManagerValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
                .EmailAddress().WithMessage("Email is not in a valid format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(255).WithMessage("Password must not exceed 255 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(255).WithMessage("Address must not exceed 255 characters.");
        }
    }
}
