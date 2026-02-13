using FluentValidation;

namespace API.DTOs.RequestDTOs.EmployeeBranches
{
    public class EmployeeBranchValidator : AbstractValidator<EmployeeBranchRequest>
    {
        public EmployeeBranchValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("BranchId is required.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(40).WithMessage("Position must not exceed 40 characters.");

            RuleFor(x => x.StartDate)
                .Must(d => d!.Value <= DateTime.Now)
                .WithMessage("StartDate cannot be in the future.")
                .When(x => x.StartDate.HasValue);
        }
    }
}
