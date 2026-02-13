using FluentValidation;

namespace API.DTOs.RequestDTOs.Accounts
{
    public class AccountValidator : AbstractValidator<AccountRequest>
    {
        public AccountValidator()
        {
            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("BranchId is required.");

            RuleFor(x => x.Iban)
                .NotEmpty().WithMessage("IBAN is required.")
                .MaximumLength(34).WithMessage("IBAN must not exceed 34 characters.")
                .Length(15, 34).WithMessage("IBAN must be between 15 and 34 characters.")
                .Matches("^[A-Z]{2}[0-9A-Z]{13,32}$").WithMessage("IBAN must be in valid format (2 letters + letters/digits).");

            RuleFor(x => x.AccountType)
                .NotEmpty().WithMessage("Account type is required.")
                .Must(t => t == "CURRENT" || t == "SAVINGS")
                .WithMessage("Account type must be CURRENT or SAVINGS.");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Balance cannot be negative.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "ACTIVE" || s == "FROZEN" || s == "CLOSED")
                .WithMessage("Status must be ACTIVE, FROZEN or CLOSED.");
        }
    }
}
