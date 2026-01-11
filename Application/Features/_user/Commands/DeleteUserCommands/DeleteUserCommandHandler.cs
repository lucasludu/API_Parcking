using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features._user.Commands.DeleteUserCommands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.guid);

            if (user == null)
                return Response<string>.Fail("Usuario no encontrado.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return Response<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return Response<string>.Success(user.Id, "Usuario eliminado exitosamente.");
        }
    }
}
