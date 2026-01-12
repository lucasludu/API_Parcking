using Application.Wrappers;
using MediatR;

namespace Application.Features._cochera.Commands.DeleteCocheraCommands
{
    public record DeleteCocheraCommand(Guid guid) : IRequest<Response<Guid>>;
}
