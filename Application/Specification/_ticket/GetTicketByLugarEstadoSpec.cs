using Ardalis.Specification;
using Domain.Entities;
using Domain.Enums;

namespace Application.Specification._ticket
{
    public class GetTicketByLugarEstadoSpec : Specification<Ticket>
    {
        public GetTicketByLugarEstadoSpec(Guid lugarId)
        {
            Query.Where(t => t.LugarId == lugarId && t.Estado == TicketEstado.Activo);
        }
    }
}
