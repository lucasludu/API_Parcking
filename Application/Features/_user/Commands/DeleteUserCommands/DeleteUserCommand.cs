using Application.Wrappers;
using MediatR;

namespace Application.Features._user.Commands.DeleteUserCommands
{
    public record DeleteUserCommand(string guid) : IRequest<Response<string>>;
}
