using FluentValidation;
using ObsidianIQ.FormsAPI.Models;

namespace ObsidianIQ.FormsAPI.Validators
{
    public class ContactFormValidator : AbstractValidator<ContactFormModel>
    {
        public ContactFormValidator()
        {
            RuleFor(x => x.ServiceRequested)
                .NotEmpty().WithMessage("Service requested is required")
                .MaximumLength(100).WithMessage("Service requested must not exceed 100 characters");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please provide a valid email address")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
                .Matches(@"^[\+]?[1-9]?[\d\s\-\(\)\.]{0,20}$").WithMessage("Please provide a valid phone number")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.CompanyName)
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters");

            RuleFor(x => x.Position)
                .MaximumLength(100).WithMessage("Position must not exceed 100 characters");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(50).WithMessage("Country must not exceed 50 characters");

            RuleFor(x => x.StateProvince)
                .NotEmpty().WithMessage("State/Province is required")
                .MaximumLength(50).WithMessage("State/Province must not exceed 50 characters");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .MinimumLength(10).WithMessage("Message must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters");
        }
    }
}
