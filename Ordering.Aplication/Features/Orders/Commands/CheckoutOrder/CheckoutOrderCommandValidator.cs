using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage("{UserName} es requerido.")
                .NotNull()
                .MaximumLength(50)
                .WithMessage("{UserName} no debe de exederce de 50 caracteres");

            RuleFor(p => p.EmailAddress)
                .NotEmpty().WithMessage("{EmailAddres} es requerido");
            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} es requerido")
                .GreaterThan(0)
                .WithMessage("{TotalPrice} el valor deberia se mayor a cero");
        }
    }
}
