using FluentValidation;

namespace Application.Features._cochera.Commands.DeleteCocheraCommands
{
    public class DeleteCocheraCommandValidator : AbstractValidator<DeleteCocheraCommand>
    {
        public DeleteCocheraCommandValidator()
        {
            RuleFor(x => x.guid)
                .NotEmpty().WithMessage("El ID de la cochera es obligatorio.")
                .NotEqual(Guid.Empty).WithMessage("El ID de la cochera no puede estar vacío.");
        }
    }
}
