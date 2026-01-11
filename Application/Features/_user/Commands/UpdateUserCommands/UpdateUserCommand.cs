using Application.Wrappers;
using MediatR;
using Models.Request._user;

namespace Application.Features._user.Commands.UpdateUserCommands
{
    public record UpdateUserCommand(string guid, UpdateUserRequest request) : IRequest<Response<string>>;
}
