using Application.Wrappers;
using MediatR;
using Models.Request._cochera;

namespace Application.Features._cochera.Commands.UpdateCocheraCommands
{
    public record UpdateCocheraCommand(Guid guid, UpdateCocheraRequest request) : IRequest<Response<Guid>>;
}
