using Application.Wrappers;
using MediatR;
using Models.Request._user;
using Models.Response._user;

namespace Application.Features._auth.Commands.LoginCommands
{
    public record LoginCommand(AuthLoginRequest Request) : IRequest<Response<LoginResponse>>;
}
