using FluentValidation;

namespace Application.Features._auth.Commands.RegisterUserCommands
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            // Validaciones de Usuario (igual que antes)
            RuleFor(p => p.Request.UserName)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacío.")
                .MaximumLength(80);

            RuleFor(p => p.Request.Email)
               .NotEmpty().EmailAddress().MaximumLength(100);

            RuleFor(p => p.Request.Password)
                .NotEmpty().MaximumLength(80);

            RuleFor(p => p.Request.ConfirmPassword)
                .Equal(p => p.Request.Password).WithMessage("Las contraseñas no coinciden.");

            // Validaciones Condicionales para Cochera
            // Si ingreso NombreCochera, asumo que quiero crear una y valido el resto
            When(x => !string.IsNullOrEmpty(x.Request.NombreCochera), () =>
            {
                RuleFor(x => x.Request.DireccionCochera)
                    .NotEmpty().WithMessage("La dirección es obligatoria si registras una cochera.");
                RuleFor(x => x.Request.CapacidadCochera)
                    .GreaterThan(0).WithMessage("La capacidad debe ser mayor a 0.");
            });
        }
    }
}
