using Application.Wrappers;
using MediatR;
using Models.Request._user;

namespace Application.Features._auth.Commands.RegisterUserCommands
{
    public record RegisterUserCommand(RegisterUserRequest Request) : IRequest<Response<string>>;
}
