using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Request._lugar;
using Models.Response._lugar;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    public class LugarController : BaseApiController
    {
        private readonly IApplicationDbContext _context;

        public LugarController(IApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/v1/Lugar
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateLugarRequest request)
        {
            var lugar = new Lugar
            {
                Identificador = request.Identificador,
                CocheraId = request.CocheraId,
                IsActive = true
            };

            _context.Lugares.Add(lugar);
            await _context.SaveChangesAsync(new CancellationToken());

            return Ok(lugar.Id);
        }

        // GET: api/v1/Lugar/ByCochera/{cocheraId}
        [HttpGet("ByCochera/{cocheraId}")]
        [Authorize]
        public async Task<ActionResult<List<LugarResponse>>> GetByCochera(Guid cocheraId)
        {
            var lugares = await _context.Lugares
                .Where(l => l.CocheraId == cocheraId && !l.Eliminado)
                .Select(l => new LugarResponse
                {
                    Id = l.Id,
                    Identificador = l.Identificador,
                    IsActive = l.IsActive,
                    // Determine status based on active tickets
                    Estado = l.Tickets.Any(t => t.Estado == Domain.Enums.TicketEstado.Activo) ? "Ocupado" : "Disponible",
                    Patente = l.Tickets.Where(t => t.Estado == Domain.Enums.TicketEstado.Activo)
                                       .Select(t => t.Patente)
                                       .FirstOrDefault()!
                })
                .ToListAsync();

            return Ok(lugares);
        }
    }

}
