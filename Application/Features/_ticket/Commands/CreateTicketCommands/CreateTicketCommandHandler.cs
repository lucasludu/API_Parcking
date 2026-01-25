using Application.Interfaces;
using Application.Specification._ticket;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features._ticket.Commands.CreateTicketCommands
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Response<Guid>>
    {
        private readonly IRepositoryAsync<Ticket> _ticketRepositoryAsync;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserService _currentUser;
        private readonly IRepositoryAsync<Lugar> _lugarRepositoryAsync;
        private readonly INotifier _notifier;

        public CreateTicketCommandHandler(IRepositoryAsync<Ticket> ticketRepositoryAsync, IDateTimeService dateTimeService, ICurrentUserService currentUser, IRepositoryAsync<Lugar> lugarRepositoryAsync, INotifier notifier)
        {
            _ticketRepositoryAsync = ticketRepositoryAsync;
            _dateTimeService = dateTimeService;
            _currentUser = currentUser;
            _lugarRepositoryAsync = lugarRepositoryAsync;
            _notifier = notifier;
        }

        public async Task<Response<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var req = request.Request;

            if (req.LugarId.HasValue)
            {
                var lugar = await _lugarRepositoryAsync.GetByIdAsync(req.LugarId.Value, cancellationToken);

                if (lugar == null)
                    return Response<Guid>.Fail("El lugar especificado no existe.");

                var ticketSpec = new GetTicketByLugarEstadoSpec(req.LugarId.Value);
                var isOccupied = await _ticketRepositoryAsync.AnyAsync(ticketSpec, cancellationToken);

                if (isOccupied)
                    return Response<Guid>.Fail("El lugar ya se encuentra ocupado.");
            }

            var ticket = new Ticket
            {
                CocheraId = req.CocheraId,
                LugarId = req.LugarId,
                Patente = req.Patente,
                TipoVehiculo = req.TipoVehiculo,
                FechaIngreso = _dateTimeService.NowUtc, // Usamos el servicio de fecha, no DateTime.Now directo
                Estado = TicketEstado.Activo,
                UsuarioEntradaId = _currentUser.UserId
            };

            try
            {
                await _ticketRepositoryAsync.AddAsync(ticket, cancellationToken);
                await _ticketRepositoryAsync.SaveChangesAsync(cancellationToken);

                await _notifier.NotifyDashboardUpdate(ticket.CocheraId); 
            }
            catch (DbUpdateConcurrencyException)
            {
                return Response<Guid>.Fail("Error de concurrencia al crear el ticket. Por favor, intente nuevamente.");
            }

            return Response<Guid>.Success(ticket.Id, "Ticket creado correctamente");
        }
    }
}
