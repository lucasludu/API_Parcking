using Application.Wrappers;
using MediatR;

namespace Application.Features._user.Commands.ToggleUserActiveCommands
{
    public record ToggleUserActiveCommand(string guid) : IRequest<Response<string>>;
}
