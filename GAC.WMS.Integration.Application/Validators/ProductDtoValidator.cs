using FluentValidation;
using GAC.WMS.Integration.Application.Models.Requests;

namespace GAC.WMS.Integration.Application.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Product Code is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Product Title is required.");

            RuleFor(x => x.Length)
                .GreaterThan(0).WithMessage("Product Length must be greater than zero.");

            RuleFor(x => x.Width)
                .GreaterThan(0).WithMessage("Product Width must be greater than zero.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Product Height must be greater than zero.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Product Weight must be greater than zero.");
        }
    }
}
