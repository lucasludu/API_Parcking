using Application.Features._ticket.Commands.CreateTicketCommands;
using Application.Features._ticket.Commands.RegistrarSalidaCommand;
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
    [ApiExplorerSettings(GroupName = "tickets")]
    [Authorize]
    public class TicketController : BaseApiController
    {
        /// <summary>
        /// Crea un nuevo Ticket de estacionamiento.
        /// </summary>
        /// <param name="request">Datos del ticket (Patente, Cochera, Lugar, etc).</param>
        // POST: api/v1/Ticket
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            // Enviamos el comando al mediador
            var result = await Mediator.Send(new CreateTicketCommand(request));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Registra la salida de un vehículo y calcula el importe final.
        /// </summary>
        /// <param name="id">ID del Ticket a cerrar.</param>
        // PUT: api/v1/Ticket/{id}/salida
        [HttpPut("{id}/salida")]
        public async Task<IActionResult> RegistrarSalida(Guid id)
        {
            var result = await Mediator.Send(new RegistrarSalidaCommand(id));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

    }

}
