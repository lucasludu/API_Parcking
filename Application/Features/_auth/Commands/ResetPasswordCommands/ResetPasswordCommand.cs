using Application.Wrappers;
using MediatR;
using Models.Request._user;

namespace Application.Features._auth.Commands.ResetPasswordCommands
{
    public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Response<string>>;
}
