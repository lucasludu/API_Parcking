using Application.Wrappers;
using MediatR;
using Models.Request._ticket;

namespace Application.Features._ticket.Commands.CreateTicketCommands
{
    public record CreateTicketCommand(CreateTicketRequest Request) : IRequest<Response<Guid>>;
}
