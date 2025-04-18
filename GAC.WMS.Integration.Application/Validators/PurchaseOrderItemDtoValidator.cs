using FluentValidation;
using GAC.WMS.Integration.Application.Models.Requests;

namespace GAC.WMS.Integration.Application.Validators
{
    public class PurchaseOrderItemDtoValidator : AbstractValidator<PurchaseOrderItemDto>
    {
        public PurchaseOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Product Code is required for each item.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit Price must be greater than zero.");
        }
    }
}
