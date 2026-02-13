using FluentValidation;

namespace API.DTOs.RequestDTOs.ManagerBranches
{
    public class ManagerBranchValidator : AbstractValidator<ManagerBranchRequest>
    {
        public ManagerBranchValidator()
        {
            RuleFor(x => x.ManagerId)
                .GreaterThan(0).WithMessage("Manager ID is required.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch ID is required.");

            RuleFor(x => x.StartDate)
                .Must(d => d!.Value <= DateTime.Now)
                .WithMessage("StartDate cannot be in the future.")
                .When(x => x.StartDate.HasValue);
        }
    }
}
