using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Request._ticket;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    public class TicketController : BaseApiController
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeService _dateTime;

        public TicketController(IApplicationDbContext context, IDateTimeService dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateTicketRequest request)
        {
            // Verify if Lugar is available
            if (request.LugarId.HasValue)
            {
                var isOccupied = await _context.Tickets
                    .AnyAsync(t => t.LugarId == request.LugarId && t.Estado == TicketEstado.Activo);
                
                if (isOccupied)
                    return BadRequest("El lugar seleccionado ya está ocupado.");
            }

            var ticket = new Ticket
            {
                CocheraId = request.CocheraId,
                LugarId = request.LugarId,
                Patente = request.Patente,
                FechaIngreso = _dateTime.NowUtc,
                Estado = TicketEstado.Activo,
                UsuarioEntradaId = "User" // TODO: Get from Claims
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(default);

            return Ok(ticket.Id);
        }

        [HttpPut("{id}/salida")]
        [Authorize]
        public async Task<IActionResult> RegistrarSalida(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            if (ticket.Estado != TicketEstado.Activo)
                return BadRequest("El ticket no está activo.");

            ticket.FechaSalida = _dateTime.NowUtc;
            ticket.Estado = TicketEstado.Pagado; // Or separate payment step? Assuming exit = paid/done for now.
            // Calculate Total? Not in requirements yet.

            await _context.SaveChangesAsync(default);
            return Ok();
        }
    }

}
