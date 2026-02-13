using FluentValidation;
using FluentValidation.AspNetCore;

namespace API.DTOs.RequestDTOs.Branches
{
    public class BranchValidator : AbstractValidator<BranchRequest>
    {
        public BranchValidator()
        {
            RuleFor(x => x.BranchName)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(60).WithMessage("Branch name must not exceed 60 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(120).WithMessage("Location must not exceed 120 characters.");
        }
    }
}
