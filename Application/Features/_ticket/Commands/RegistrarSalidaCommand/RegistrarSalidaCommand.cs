using Application.Wrappers;
using MediatR;

namespace Application.Features._ticket.Commands.RegistrarSalidaCommand
{
    public record RegistrarSalidaCommand(Guid Id) : IRequest<Response<Guid>>;
}
