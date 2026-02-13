using FluentValidation;

namespace API.DTOs.RequestDTOs.EmployeeAccountPermissions
{
    public class EmployeeAccountPermissionValidator : AbstractValidator<EmployeeAccountPermissionRequest>
    {
        public EmployeeAccountPermissionValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee ID is required.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Account ID is required.");

            RuleFor(x => x.Permission)
                .NotEmpty().WithMessage("Permission is required.")
                .MaximumLength(10).WithMessage("Permission must not exceed 10 characters.")
                .Must(p => p == "READ" || p == "WRITE")
                .WithMessage("Permission must be READ or WRITE.");

            RuleFor(x => x.GrantedAt)
                .Must(d => d!.Value <= DateTime.Now)
                .WithMessage("Granted at cannot be in the future.")
                .When(x => x.GrantedAt.HasValue);
        }
    }
}
