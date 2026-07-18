using FluentValidation;

namespace Application.Features._ticket.Commands.CreateTicketCommands
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.Request.Patente)
                .NotEmpty().WithMessage("La patente es obligatoria.")
                .MaximumLength(10).WithMessage("La patente no puede tener más de 10 caracteres.")
                .Matches(@"^[A-Z0-9]+$").WithMessage("La patente solo puede contener letras mayúsculas y números.");

            RuleFor(x => x.Request.CocheraId)
                .NotEmpty().WithMessage("El ID de la cochera es obligatorio.");

            RuleFor(x => x.Request.TipoVehiculo)
                .IsInEnum().WithMessage("El tipo de vehículo no es válido.");

            // Validación Condicional: Si envía LugarId, no puede ser vacío (aunque sea nullable en el DTO, si lo manda, que sea válido)
            RuleFor(x => x.Request.LugarId)
               .Must(id => id != Guid.Empty).When(x => x.Request.LugarId.HasValue)
               .WithMessage("El ID del lugar no es válido.");
        }
    }
}
