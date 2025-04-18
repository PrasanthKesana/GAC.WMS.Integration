using FluentValidation;
using GAC.WMS.Integration.Application.Models.Requests;

namespace GAC.WMS.Integration.Application.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator()
        {
            RuleFor(x => x.CustomerIdentifier)
                .NotEmpty().WithMessage("Customer Identifier is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Customer Name is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Customer Address is required.");
        }
    }
}
