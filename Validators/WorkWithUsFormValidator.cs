using FluentValidation;
using ObsidianIQ.FormsAPI.Models;

namespace ObsidianIQ.FormsAPI.Validators
{
    public class WorkWithUsFormValidator : AbstractValidator<WorkWithUsFormModel>
    {
        public WorkWithUsFormValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please provide a valid email address")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
                .Matches(@"^[\+]?[1-9]?[\d\s\-\(\)]{7,15}$").WithMessage("Please provide a valid phone number")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .MinimumLength(10).WithMessage("Message must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters");

            // HasResume is optional - no validation needed for boolean
        }
    }
}