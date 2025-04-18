using FluentValidation;
using GAC.WMS.Integration.Application.Models.Requests;

namespace GAC.WMS.Integration.Application.Validators
{
    public class SalesOrderDtoValidator : AbstractValidator<SalesOrderDto>
    {
        public SalesOrderDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Sales Order ID is required.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer ID must be greater than zero.");

            RuleFor(x => x.ShipmentAddress)
                .NotEmpty().WithMessage("Shipment Address is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item is required.")
                .ForEach(x => x.SetValidator(new SalesOrderItemDtoValidator()));
        }
    }
}
