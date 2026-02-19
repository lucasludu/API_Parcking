using FluentValidation;

namespace Application.Features._cochera.Commands.UpdateCocheraCommands
{
    public class UpdateCocheraCommandValidator : AbstractValidator<UpdateCocheraCommand>
    {
        public UpdateCocheraCommandValidator()
        {
            RuleFor(x => x.guid)
                .NotEmpty().WithMessage("El ID de la cochera es obligatorio.")
                .NotEqual(Guid.Empty).WithMessage("El ID de la cochera no puede estar vacío.");

            RuleFor(x => x.request.Nombre)
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .When(x => x.request.Nombre != null);

            RuleFor(x => x.request.CapacidadTotal)
                .GreaterThan(0).WithMessage("La capacidad total debe ser mayor a 0.")
                .When(x => x.request.CapacidadTotal.HasValue);
        }
    }
}
