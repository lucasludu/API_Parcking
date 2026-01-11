using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features._user.Commands.ToggleUserActiveCommands
{
    public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ToggleUserActiveCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.guid);

            if (user == null)
                return Response<string>.Fail("Usuario no encontrado.");

            // Verificamos si está bloqueado actualmente
            bool isLocked = await _userManager.IsLockedOutAsync(user);

            if (isLocked)
            {
                // DESBLOQUEAR: Establecemos el fin del bloqueo a "null" (o fecha pasada)
                await _userManager.SetLockoutEndDateAsync(user, null);
                return Response<string>.Success(user.Id, "Usuario reactivado exitosamente. Ahora puede iniciar sesión.");
            }
            else
            {
                // BLOQUEAR: Establecemos el fin del bloqueo al máximo posible (año 9999)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                return Response<string>.Success(user.Id, "Usuario desactivado/bloqueado exitosamente.");
            }
        }
    }
}
