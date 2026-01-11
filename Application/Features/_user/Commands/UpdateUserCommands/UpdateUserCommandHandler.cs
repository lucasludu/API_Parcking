using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features._user.Commands.UpdateUserCommands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.guid);

            if (user == null)
                return Response<string>.Fail("Usuario no encontrado.");

            // Actualizamos los campos si vienen en el request
            if (!string.IsNullOrEmpty(request.request.UserName))
                user.UserName = request.request.UserName;

            if (!string.IsNullOrEmpty(request.request.PhoneNumber))
                user.PhoneNumber = request.request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Response<string>.Fail(result.Errors.Select(e => e.Description).ToList());

            return Response<string>.Success(user.Id, "Usuario actualizado exitosamente.");
        }
    }
}
