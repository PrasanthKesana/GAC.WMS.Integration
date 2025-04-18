using FluentValidation;
using GAC.WMS.Integration.Application.Models.Requests;

namespace GAC.WMS.Integration.Application.Validators
{
    public class PurchaseOrderDtoValidator : AbstractValidator<PurchaseOrderDto>
    {
        public PurchaseOrderDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Purchase Order ID is required.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer ID must be greater than zero.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item is required.")
                .ForEach(x => x.SetValidator(new PurchaseOrderItemDtoValidator()));
        }
    }
}
