using Application.Interfaces;
using Application.Specification._cochera;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features._user.Commands.DeleteUserCommands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepositoryAsync<Cochera> _cocheraRepositoryAsync;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager, IRepositoryAsync<Cochera> cocheraRepositoryAsync)
        {
            _userManager = userManager;
            _cocheraRepositoryAsync = cocheraRepositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.guid);

            if (user == null)
                return Response<string>.Fail("Usuario no encontrado.");

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                if (user.CocheraId != null)
                {
                    var empleados = _userManager.Users
                        .Where(u => u.CocheraId == user.CocheraId && u.Id != user.Id).ToList();

                    foreach(var empleado in empleados)
                    {
                        var removeEmpleados = await _userManager.DeleteAsync(empleado);
                        
                        if(!removeEmpleados.Succeeded)
                            return Response<string>.Fail("Error al eliminar los empleados asociados a la cochera del usuario.");
                    }

                    var cocheraSpec = new GetCocheraByIdSpec((Guid)user.CocheraId!);
                    var cochera = await _cocheraRepositoryAsync.FirstOrDefaultAsync(cocheraSpec, cancellationToken);

                    if (cochera != null)
                    {
                        var deletedCochera = await _cocheraRepositoryAsync.DeleteAsync(cochera, cancellationToken);
                        if (deletedCochera == 0)
                            return Response<string>.Fail("Error al eliminar la cochera asociada al usuario.");
                    } 
                }
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return Response<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return Response<string>.Success(user.Id, "Usuario eliminado exitosamente.");
        }
    }
}
