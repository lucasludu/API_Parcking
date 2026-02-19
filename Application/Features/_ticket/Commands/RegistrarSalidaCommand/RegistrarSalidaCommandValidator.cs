using FluentValidation;

namespace Application.Features._ticket.Commands.RegistrarSalidaCommand
{
    public class RegistrarSalidaCommandValidator : AbstractValidator<RegistrarSalidaCommand>
    {
        public RegistrarSalidaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID del ticket es obligatorio.")
                .NotEqual(Guid.Empty).WithMessage("El ID del ticket no puede estar vacío.");
        }
    }
}
