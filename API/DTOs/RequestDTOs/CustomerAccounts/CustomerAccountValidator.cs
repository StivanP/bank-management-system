using FluentValidation;

namespace API.DTOs.RequestDTOs.CustomerAccounts
{
    public class CustomerAccountValidator : AbstractValidator<CustomerAccountRequest>
    {
        public CustomerAccountValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer ID is required.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Account ID is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .MaximumLength(10).WithMessage("Role must not exceed 10 characters.")
                .Must(r => r == "PRIMARY" || r == "JOINT")
                .WithMessage("Role must be PRIMARY or JOINT.");

            RuleFor(x => x.SinceDate)
                .Must(d => d!.Value <= DateTime.Now)
                .WithMessage("Since Date cannot be in the future.")
                .When(x => x.SinceDate.HasValue);
        }
    }
}
