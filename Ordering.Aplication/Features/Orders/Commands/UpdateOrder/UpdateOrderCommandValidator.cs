using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
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
