using Application.Interfaces;
using Application.Specification._tarifa;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features._ticket.Commands.RegistrarSalidaCommand
{
    public class RegistrarSalidaCommandHandler : IRequestHandler<RegistrarSalidaCommand, Response<Guid>>
    {
        private readonly IRepositoryAsync<Ticket> _ticketRepositoryAsync;
        private readonly IRepositoryAsync<Tarifa> _tarifaRepository;
        private readonly IDateTimeService _datetimeService;

        public RegistrarSalidaCommandHandler(IRepositoryAsync<Ticket> ticketRepositoryAsync, IDateTimeService datetimeService, IRepositoryAsync<Tarifa> tarifaRepository)
        {
            _ticketRepositoryAsync = ticketRepositoryAsync;
            _datetimeService = datetimeService;
            _tarifaRepository = tarifaRepository;
        }

        public async Task<Response<Guid>> Handle(RegistrarSalidaCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepositoryAsync.GetByIdAsync(request.Id);

            if (ticket == null)
                return Response<Guid>.Fail("Ticket no encontrado");

            if (ticket.Estado != TicketEstado.Activo)
                return Response<Guid>.Fail("El ticket ya fue cerrado o no está activo.");

            ticket.FechaSalida = _datetimeService.NowUtc;
            ticket.Estado = TicketEstado.Pagado;

            var spec = new TarifaVigenteSpec(ticket.CocheraId, ticket.TipoVehiculo); 
            var tarifa = await _tarifaRepository.FirstOrDefaultAsync(spec, cancellationToken);
            
            ticket.Total = CalcularTotal(ticket.FechaIngreso, ticket.FechaSalida.Value, tarifa);
            await _tarifaRepository.UpdateAsync(tarifa!, cancellationToken);

            return Response<Guid>.Success(ticket.Id, $"Salida registrada. Total a cobrar: ${ticket.Total}");
        }


        #region Métodos Privados

        private decimal CalcularTotal(DateTime ingreso, DateTime salida, Tarifa? tarifa)
        {
            // Si no hay tarifa configurada, cobramos 0 (o podrías lanzar excepción según negocio)
            if (tarifa == null) return 0;

            var duracion = salida - ingreso;

            // Regla: Cobrar horas completas (redondeo hacia arriba)
            // Math.Ceiling devuelve double, casteamos a int o decimal
            var horasACobrar = (int)Math.Ceiling(duracion.TotalHours);

            // Regla: Mínimo 1 hora de cobro
            if (horasACobrar <= 0) horasACobrar = 1;

            decimal totalCalculado = horasACobrar * tarifa.PrecioHora;

            // Regla: Aplicar tope de Estadía si corresponde
            if (tarifa.PrecioEstadia > 0 && totalCalculado > tarifa.PrecioEstadia)
                totalCalculado = tarifa.PrecioEstadia;

            return totalCalculado;
        }

        #endregion

    }
}
